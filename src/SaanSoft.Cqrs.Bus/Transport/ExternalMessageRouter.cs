using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.Transport;

public sealed class ExternalMessageRouter(
        ILogger<ExternalMessageRouter> logger,
        IExternalMessageProvider externalMessageProvider,
        IDefaultExternalMessageRouterOptions? messageRouterOptions = null,
        IReadOnlyCollection<IMiddleware>? externalPublisherMiddlewares = null
    ) : IMessageRouter
{
    private readonly string _externalMessageProviderName = "External transport";
    private readonly IMiddleware[] _middlewares
        = externalPublisherMiddlewares?.ToArray() ?? [];

    private readonly IDefaultExternalMessageRouterOptions _messageRouterOptions
        = messageRouterOptions ?? new DefaultExternalMessageRouterOptions();

    public Task ExecuteAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
        => PublishManyExternallyAsync<TMessage>([envelope], true, ct);

    public Task<TResponse> ExecuteAsync<TMessage, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage<TResponse>
        => PublishExternallyAndWaitForResponseAsync<TMessage, TResponse>(envelope, ct);

    public Task SendAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
        => PublishManyExternallyAsync<TMessage>([envelope], false, ct);

    public Task SendManyAsync<TMessage>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TMessage : IMessage
        => PublishManyExternallyAsync<TMessage>(envelopes, false, ct);

    /// <summary>
    /// Publish one or more messages via the IExternalMessageProvider
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="envelopes"></param>
    /// <param name="waitForExecution"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    private async Task PublishManyExternallyAsync<TMessage>(
        MessageEnvelope[] envelopes, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
    {
        if (envelopes.Length == 0) return;

        var firstMessage = envelopes.First().Message;
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        var options = _messageRouterOptions.Clone(waitForExecution, expectSingleHandler: firstMessage is not IEvent);

        if (firstMessage is ITimeout timeoutMessage)
        {
            var timeoutMessages = envelopes.Select(e => (ITimeout)e.Message);
            options.Timeout = timeoutMessages.MaxBy(e => e.Timeout)?.Timeout ?? timeoutMessage.Timeout;
        }
        using var timeoutCts = new CancellationTokenSource(options.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var transportCt = linkedCts.Token;

            // run the transport pipeline over each envelope
            var tasks = envelopes.Select(envelope => _middlewares.InvokeAsync<TMessage>(envelope, ct));
            await Task.WhenAll(tasks);

            logger.LogDebug("Start: Publishing {MessageType}", messageTypeName);
            var response = await externalMessageProvider.PublishManyAsync(envelopes, options, transportCt);
            if (response is { Success: true })
            {
                logger.LogDebug("Stop: Successfully published {MessageType}", messageTypeName);
                return;
            }

            // handler threw an exception - mark failed and rethrow
            if (response?.Exception != null)
            {
                logger.LogError(response.Exception, "Failed to process {MessageType}", messageTypeName);
                foreach (var envelope in envelopes)
                {
                    envelope.MarkFailed(_externalMessageProviderName, response.Exception);
                }
                throw response.Exception;
            }

            var errorMessage = response?.ErrorMessage ?? "External publish message failed";
            if (response == null)
            {
                errorMessage = "External publish message failed: External transport did not return a response";
            }

            throw new ApplicationException(errorMessage);
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException(
                $"External publish message(s) failed: Timed out after {options.Timeout}.",
                ex
            );
            logger.LogError(
                exception,
                "Failed to process {MessageType}: {Reason}",
                messageTypeName,
                $"Timed out after {options.Timeout}"
            );

            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageProviderName, exception);
            }
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageProviderName, ex);
            }
            throw;
        }
    }

    private async Task<TResponse> PublishExternallyAndWaitForResponseAsync<TMessage, TResponse>(
        MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        // if we want a response, we have to wait for the execution
        var options = _messageRouterOptions.Clone(waitForExecution: true, expectSingleHandler: true);
        if (envelope.Message is ITimeout timeoutMessage)
            options.Timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(options.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var transportCt = linkedCts.Token;

            await _middlewares.InvokeAsync<TMessage>(envelope, transportCt);
            var response = await externalMessageProvider!.PublishAndWaitForResponseAsync<TResponse>(
                envelope,
                options,
                transportCt
            );
            if (response?.Exception != null)
            {
                logger.LogError(response.Exception, "Failed to process {MessageType}", messageTypeName);
                envelope.MarkFailed(_externalMessageProviderName, response.Exception);
                throw response.Exception;
            }

            var errorMessage = response?.ErrorMessage ?? "External publish message failed";
            if (response != null && response.Success)
            {
                if (response.Payload != null) return response.Payload;

                var returnType = typeof(TResponse);
                // Reference types or Nullable<T> are allowed to return null
                if (!returnType.IsValueType || Nullable.GetUnderlyingType(returnType) != null)
                {
                    // null for reference/nullable, compiler-safe
                    return default!;
                }

                // Success=true, but Payload is null and TResponse is not nullable
                // populate errorMessage for use below
                errorMessage = @$"Response from transport was marked as a success, but " +
                    $"{nameof(ExternalResponse<TResponse>.Payload)} was null and {returnType.GetTypeFullName()} is " +
                    $"not nullable.";
                envelope.MarkFailed(_externalMessageProviderName, errorMessage);
            }

            if (response == null)
            {
                errorMessage = $"External publish message failed: External transport did not return a response";
            }

            throw new ApplicationException(errorMessage);
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException(
                $"External publish message failed: Timed out after {options.Timeout}.",
                ex
            );
            logger.LogError(
                exception,
                "Failed to process {MessageType}: {Reason}",
                messageTypeName,
                $"Timed out after {options.Timeout}"
            );
            envelope.MarkFailed(_externalMessageProviderName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            envelope.MarkFailed(_externalMessageProviderName, ex);
            throw;
        }
    }
}
