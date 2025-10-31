using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.Transport;

public sealed class ExternalMessageProcessor(
        ILogger<ExternalMessageProcessor> logger,
        IDefaultProcessorOptions? defaultProcessorOptions,
        IExternalMessageBroker externalMessageBroker,
        IMiddleware[]? externalPublisherMiddlewares
    ) : IMessageRouter
{
    private readonly string _externalMessageBrokerName = "External transport";
    private readonly IMiddleware[] _middlewares
        = externalPublisherMiddlewares ?? [];

    private readonly IDefaultProcessorOptions _defaultProcessorOptions
        = defaultProcessorOptions ?? new DefaultProcessorOptions();

    public Task ExecuteAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand
        => PublishExternallyAndWaitAsync<TCommand>(envelope, ct);

    public Task<TResponse> ExecuteAsync<TCommand, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand<TResponse>
        => PublishExternallyAndWaitForResponseAsync<TCommand, TResponse>(envelope, ct);

    public Task SendAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand
        => PublishExternallyAsync<TCommand>([envelope], ct);

    public Task<TResponse> QueryAsync<TQuery, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TQuery : IQuery<TResponse>
        => PublishExternallyAndWaitForResponseAsync<TQuery, TResponse>(envelope, ct);

    public Task PublishManyAsync<TEvent>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TEvent : IEvent
        => PublishExternallyAsync<TEvent>(envelopes, ct);

    private Task PublishExternallyAndWaitAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage => PublishExternallyAsync<TMessage>([envelope], true, ct);

    private Task PublishExternallyAsync<TMessage>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TMessage : IMessage => PublishExternallyAsync<TMessage>(envelopes, false, ct);

    private async Task PublishExternallyAsync<TMessage>(
        MessageEnvelope[] envelopes, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
    {
        if (envelopes.Length == 0) return;

        var firstMessage = envelopes.First().Message;
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        var options = _defaultProcessorOptions.Clone(waitForExecution, expectSingleHandler: firstMessage is not IEvent);

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
            var response = await externalMessageBroker.PublishManyAsync(envelopes, options, transportCt);
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
                    envelope.MarkFailed(_externalMessageBrokerName, response.Exception);
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
                envelope.MarkFailed(_externalMessageBrokerName, exception);
            }
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageBrokerName, ex);
            }
            throw;
        }
    }

    public async Task<TResponse> PublishExternallyAndWaitForResponseAsync<TMessage, TResponse>(
        MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        // if we want a response, we have to wait for the execution
        var options = _defaultProcessorOptions.Clone(waitForExecution: true, expectSingleHandler: true);
        if (envelope.Message is ITimeout timeoutMessage)
            options.Timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(options.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var transportCt = linkedCts.Token;

            await _middlewares.InvokeAsync<TMessage>(envelope, transportCt);
            var response = await externalMessageBroker!.PublishAsync(envelope, options, transportCt);
            if (response?.Exception != null)
            {
                logger.LogError(response.Exception, "Failed to process {MessageType}", messageTypeName);
                envelope.MarkFailed(_externalMessageBrokerName, response.Exception);
                throw response.Exception;
            }

            var errorMessage = response?.ErrorMessage ?? "External publish message failed";
            if (response != null && response.Success)
            {
                if (response.Payload != null) return (TResponse)response.Payload;

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
                    $"{nameof(ExternalResponse.Payload)} was null and {returnType.GetTypeFullName()} is not nullable.";
                envelope.MarkFailed(_externalMessageBrokerName, errorMessage);
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
            envelope.MarkFailed(_externalMessageBrokerName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            envelope.MarkFailed(_externalMessageBrokerName, ex);
            throw;
        }
    }
}
