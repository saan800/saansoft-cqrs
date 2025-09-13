using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.InMemory;

public sealed class InMemoryMessageProcessor(
        ILogger<InMemoryMessageProcessor> logger,
        IServiceRegistry serviceRegistry,
        IDefaultProcessorOptions? defaultProcessorOptions,
        IEnumerable<ISubscriberMiddleware<IMessage>>? subscriberMiddleware) : IInMemoryMessageProcessor
{
    private readonly IDefaultProcessorOptions _defaultProcessorOptions
        = defaultProcessorOptions ?? new DefaultProcessorOptions();

    public async Task HandleCommandEnvelopeAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand
    {
        var command = (TCommand)envelope.Message;
        var (handlerType, handler) = GetHandlerInfo(command);
        using (logger.BeginScope(command.BuildLoggingScopeData(handlerType)))
        {
            await RunSubscriberPipeline<TCommand>(
                envelope,
                handlerType,
                () => HandleMessageInMemoryAsync(command, envelope, handlerType, handler, ct),
                ct
            );
        }
    }

    public async Task<TResult> HandleCommandEnvelopeAsync<TCommand, TResult>(
        MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand<TResult>
    {
        var command = (TCommand)envelope.Message;
        var (handlerType, handler) = GetHandlerInfo(command);
        using (logger.BeginScope(command.BuildLoggingScopeData(handlerType)))
        {
            return await RunSubscriberPipelineWithResult<TCommand, TResult>(
                envelope,
                handlerType,
                () => HandleMessageInMemoryWithResultAsync<TCommand, TResult>(
                    command, envelope, handlerType, handler, ct),
                ct
            );
        }
    }

    public async Task HandleEventEnvelopesAsync<TEvent>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TEvent : IEvent
    {
        if (envelopes.Length == 0) return;

        var firstEvent = (TEvent)envelopes.First().Message;
        var handlerType = typeof(IEventHandler<>).MakeGenericType(firstEvent.GetType());
        var handlers = serviceRegistry.ResolveMultipleHandlers(handlerType).ToArray();
        foreach (var handler in handlers)
        {
            foreach (var envelope in envelopes)
            {
                var evt = (IEvent)envelope.Message;
                using (logger.BeginScope(evt.BuildLoggingScopeData(handlerType)))
                {
                    var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
                    envelope.MarkPending(handlerName);

                    await RunSubscriberPipeline<TEvent>(
                        envelope,
                        handlerType,
                        () => HandleMessageInMemoryAsync(evt, envelope, handlerType, handler, ct),
                        ct
                    );
                }
            }
        }
    }

    public async Task<TResult> HandleQueryEnvelopeAsync<TQuery, TResult>(
        MessageEnvelope envelope, CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        var query = (TQuery)envelope.Message;
        var (handlerType, handler) = GetHandlerInfo(query);
        using (logger.BeginScope(query.BuildLoggingScopeData(handlerType)))
        {
            return await RunSubscriberPipelineWithResult<TQuery, TResult>(
                envelope,
                handlerType,
                () => HandleMessageInMemoryWithResultAsync<TQuery, TResult>(
                    query, envelope, handlerType, handler, ct),
                ct
            );
        }
    }

    /// <summary>
    /// Get the handler type and instance for the given message's handler
    /// </summary>
    /// <remarks>
    /// Works for messages other than IEvent, which can have multiple handlers
    /// </remarks>
    private (Type handlerType, object handler) GetHandlerInfo<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        // TODO: can we get the actual handler instance with generic type?
        var messageType = message.GetType();
        var openGenericHandlerType = message switch
        {
            var _ when messageType.ImplementsGeneric(typeof(ICommand<>)) => typeof(ICommandHandler<,>),
            var _ when messageType.ImplementsGeneric(typeof(IQuery<>)) => typeof(IQueryHandler<,>),
            ICommand => typeof(ICommandHandler<>),
            IEvent => throw new NotSupportedException(
                $"Can have multiple IEventHandler<>, use PublishAsync<TEvent> instead"),
            _ => throw new NotSupportedException($"Unsupported message type: {messageType.GetTypeFullName()}"),
        };

        var handlerType = openGenericHandlerType.MakeGenericType(messageType);
        var handler = serviceRegistry.ResolveSingleHandler(handlerType)
            // shouldn't happen as routing said it found InMemory handler, but makes the compiler happy
            ?? throw new ApplicationException($"No in-memory handler found for {messageType.GetTypeFullName()}");

        return (handlerType, handler);
    }

    private Task RunSubscriberPipeline<TMessage>(
        MessageEnvelope envelope, Type handlerType, Func<Task> terminal, CancellationToken ct)
        where TMessage : IMessage
    {
        envelope.MarkPending(handlerType.GetTypeFullName());
        var ctx = new HandlerContext(envelope, handlerType, serviceRegistry);

        var validMiddlewares = subscriberMiddleware
            .GetValidMiddlewares<ISubscriberMiddleware<TMessage>, ISubscriberMiddleware<IMessage>>();
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        return MiddlewareExtensions.RunPipeline(middlewares, terminal);
    }

    private async Task<TResult> RunSubscriberPipelineWithResult<TMessage, TResult>(
        MessageEnvelope envelope, Type handlerType, Func<Task<TResult>> terminal, CancellationToken ct)
        where TMessage : IMessage
    {
        envelope.MarkPending(handlerType.GetTypeFullName());
        var ctx = new HandlerContext(envelope, handlerType, serviceRegistry);

        TResult result = default!;
        async Task NewTerminal() => result = await terminal();

        var validMiddlewares = subscriberMiddleware
            .GetValidMiddlewares<ISubscriberMiddleware<TMessage>, ISubscriberMiddleware<IMessage>>();
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        await MiddlewareExtensions.RunPipeline(middlewares, NewTerminal);
        return result;
    }

    /// <summary>
    /// Handle the message in memory
    /// </summary>
    private async Task HandleMessageInMemoryAsync<TMessage>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage
    {
        var timeout = _defaultProcessorOptions.Timeout;
        if (envelope.Message is ITimeout timeoutMessage)
            timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var messageTypeName = typeof(TMessage).GetTypeFullName();
        var handlerTypeName = handlerType.GetTypeFullName();
        try
        {
            logger.LogDebug("Start: Handle the {MessageType} by {HandlerType}", messageTypeName, handlerTypeName);
            await (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, linkedCts.Token])!;
            envelope.MarkSuccess(handlerTypeName);
            logger.LogDebug(
                "Stop: Handled the {MessageType} by {HandlerType} successfully", messageTypeName, handlerTypeName);
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException($"Handler {handlerTypeName} timed out after {timeout}.", ex);
            logger.LogError(
                exception,
                "{HandlerType} failed to handle {MessageType}: {Reason}",
                handlerTypeName,
                messageTypeName,
                $"Timed out after {timeout}"
            );
            envelope.MarkFailed(handlerTypeName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{HandlerType} failed to handle {MessageType}",
                handlerTypeName,
                messageTypeName
            );
            envelope.MarkFailed(handlerTypeName, ex);
            throw;
        }
    }

    /// <summary>
    /// Handle the message in memory and return the result
    /// </summary>
    private async Task<TResult> HandleMessageInMemoryWithResultAsync<TMessage, TResult>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage<TResult>
    {
        var timeout = _defaultProcessorOptions.Timeout;
        if (envelope.Message is ITimeout timeoutMessage)
            timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var messageTypeName = typeof(TMessage).GetTypeFullName();
        var handlerTypeName = handlerType.GetTypeFullName();
        try
        {
            var task = (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, linkedCts.Token])!;
            await task.ConfigureAwait(false);
            // Task<TResult> result extraction
            var t = (dynamic)task;
            var result = (TResult)t.GetAwaiter().GetResult();
            envelope.MarkSuccess(handlerTypeName);
            return result;
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException($"Handler {handlerTypeName} timed out after {timeout}.", ex);
            logger.LogError(
                exception,
                "{HandlerType} failed to handle {MessageType}: {Reason}",
                handlerTypeName,
                messageTypeName,
                $"Timed out after {timeout}"
            );
            envelope.MarkFailed(handlerTypeName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{HandlerType} failed to handle {MessageType}",
                handlerTypeName,
                messageTypeName
            );
            envelope.MarkFailed(handlerTypeName, ex);
            throw;
        }
    }
}
