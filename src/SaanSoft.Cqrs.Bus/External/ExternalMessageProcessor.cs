using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.External;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.InMemory;

public sealed class ExternalMessageProcessor(
        ILogger<ExternalMessageProcessor> logger,
        IServiceRegistry serviceRegistry,
        IDefaultProcessorOptions? defaultProcessorOptions,
        IExternalMessageTransport externalMessageTransport,
        IEnumerable<IExternalTransportPublisherMiddleware<IMessage>>? externalTransportPublisherMiddleware
    ) : IExternalMessageProcessor
{
    private readonly string _externalMessageTransportName = "External transport";

    private readonly IDefaultProcessorOptions _defaultProcessorOptions
        = defaultProcessorOptions ?? new DefaultProcessorOptions();

    public Task PublishExternallyAndWaitAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage => PublishExternallyAsync<TMessage>([envelope], true, ct);

    public Task PublishExternallyAsync<TMessage>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TMessage : IMessage => PublishExternallyAsync<TMessage>(envelopes, false, ct);

    private async Task PublishExternallyAsync<TMessage>(
        MessageEnvelope[] envelopes, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
    {
        if (envelopes.Length == 0) return;

        var firstMessage = envelopes.First().Message;
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        var options = _defaultProcessorOptions.Clone(waitForExecution);
        options.ExpectSingleHandler = firstMessage is not IEvent;

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
            var tasks = envelopes.Select(envelope => RunTransportPipeline<TMessage>(envelope, ct));
            await Task.WhenAll(tasks);

            logger.LogDebug("Start: Publishing {MessageType}", messageTypeName);
            var result = await externalMessageTransport.PublishManyAsync(envelopes, options, transportCt);
            if (result is { Success: true })
            {
                logger.LogDebug("Stop: Successfully published {MessageType}", messageTypeName);
                return;
            }

            // handler threw an exception - mark failed and rethrow
            if (result?.Exception != null)
            {
                logger.LogError(result.Exception, "Failed to process {MessageType}", messageTypeName);
                foreach (var envelope in envelopes)
                {
                    envelope.MarkFailed(_externalMessageTransportName, result.Exception);
                }
                throw result.Exception;
            }

            var errorMessage = result?.ErrorMessage ?? "External publish message failed";
            if (result == null)
            {
                errorMessage = $"External publish message failed: External transport did not return a result";
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
                envelope.MarkFailed(_externalMessageTransportName, exception);
            }
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageTransportName, ex);
            }
            throw;
        }
    }

    public async Task<TResult> PublishExternallyAndWaitForResultsAsync<TMessage, TResult>(
        MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var messageTypeName = typeof(TMessage).GetTypeFullName();

        // if we want a response, we have to wait for the execution
        var options = _defaultProcessorOptions.Clone(waitForExecution: true);
        options.ExpectSingleHandler = true;
        if (envelope.Message is ITimeout timeoutMessage)
            options.Timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(options.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var transportCt = linkedCts.Token;

            await RunTransportPipeline<TMessage>(envelope, transportCt);
            var result = await externalMessageTransport!.PublishAsync(envelope, options, transportCt);
            if (result?.Exception != null)
            {
                logger.LogError(result.Exception, "Failed to process {MessageType}", messageTypeName);
                envelope.MarkFailed(_externalMessageTransportName, result.Exception);
                throw result.Exception;
            }

            var errorMessage = result?.ErrorMessage ?? "External publish message failed";
            if (result != null && result.Success)
            {
                if (result.Payload != null) return (TResult)result.Payload;

                var returnType = typeof(TResult);
                // Reference types or Nullable<T> are allowed to return null
                if (!returnType.IsValueType || Nullable.GetUnderlyingType(returnType) != null)
                {
                    // null for reference/nullable, compiler-safe
                    return default!;
                }

                // Success=true, but Payload is null and TResult is not nullable
                // populate errorMessage for use below
                errorMessage = @$"Result from transport was marked as a success, but " +
                     $"{nameof(ExternalResult.Payload)} was null and {returnType.GetTypeFullName()} is not nullable.";
                envelope.MarkFailed(_externalMessageTransportName, errorMessage);
            }

            if (result == null)
            {
                errorMessage = $"External publish message failed: External transport did not return a result";
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
            envelope.MarkFailed(_externalMessageTransportName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {MessageType}", messageTypeName);
            envelope.MarkFailed(_externalMessageTransportName, ex);
            throw;
        }
    }

    private Task RunTransportPipeline<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var ctx = new TransportContext(envelope, serviceRegistry);
        static Task Terminal() => Task.CompletedTask;

        var validMiddlewares = externalTransportPublisherMiddleware
            .GetValidMiddlewares<
                IExternalTransportPublisherMiddleware<TMessage>, IExternalTransportPublisherMiddleware<IMessage>>();
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        return MiddlewareExtensions.RunPipeline(middlewares, Terminal);
    }
}
