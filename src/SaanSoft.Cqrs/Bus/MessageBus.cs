
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public sealed class MessageBus(
    IServiceRegistry serviceRegistry,
    IRoutingStrategy routing,
    IDefaultExternalTransportOptions? defaultExternalTransportOptions,
    IEnumerable<IPublisherMiddleware<IMessage>>? publisherMiddleware,
    IEnumerable<ISubscriberMiddleware<IMessage>>? subscriberMiddleware,
    IExternalMessageTransport? externalMessageTransport,
    IEnumerable<IExternalTransportPublisherMiddleware<IMessage>>? externalTransportPublisherMiddleware) : IMessageBus
{
    private readonly IDefaultExternalTransportOptions _defaultExternalTransportOptions
        = defaultExternalTransportOptions ?? new DefaultExternalTransportOptions();

    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        var envelope = MessageEnvelope.Wrap(command);
        await RunPublisherPipeline<TCommand>(envelope, ct);

        if (routing.IsExternalMessage(command))
        {
            await PublishExternallyAsync<TCommand>(envelope, waitForExecution: true, ct);
            return;
        }

        // handle InMemory
        var (handlerType, handler) = GetHandlerInfo(command);
        await RunSubscriberPipeline<TCommand>(
            envelope,
            handlerType,
            () => HandleMessageInMemoryAsync(command, envelope, handlerType, handler, ct),
            ct
        );
    }

    public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResult>
    {
        var envelope = MessageEnvelope.Wrap(command);
        await RunPublisherPipeline<TCommand>(envelope, ct);

        if (routing.IsExternalMessage(command))
        {
            return await PublishExternallyWithResultAsync<TCommand, TResult>(envelope, ct);
        }

        // handle InMemory
        var (handlerType, handler) = GetHandlerInfo(command);
        return await RunSubscriberPipelineWithResult<TCommand, TResult>(
            envelope,
            handlerType,
            () => HandleMessageInMemoryWithResultAsync<TCommand, TResult>(command, envelope, handlerType, handler, ct),
            ct
        );
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        var envelope = MessageEnvelope.Wrap(command);
        await RunPublisherPipeline<TCommand>(envelope, ct);

        if (routing.IsExternalMessage(command))
        {
            await PublishExternallyAsync<TCommand>(envelope, waitForExecution: false, ct);
            return;
        }

        // handle InMemory
        var (handlerType, handler) = GetHandlerInfo(command);
        await RunSubscriberPipeline<TCommand>(
            envelope,
            handlerType,
            () => HandleMessageInMemoryAsync(command, envelope, handlerType, handler, ct),
            ct
        );
    }

    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent
    {
        var envelope = MessageEnvelope.Wrap(evt);
        await RunPublisherPipeline<TEvent>(envelope, ct);

        if (routing.IsExternalMessage(evt))
        {
            await PublishExternallyAsync<TEvent>(envelope, waitForExecution: false, ct);
            return;
        }

        // handle InMemory
        var handlerType = typeof(IEventHandler<>).MakeGenericType(evt.GetType());
        var handlers = serviceRegistry.ResolveMultipleHandlers(handlerType).ToArray();
        foreach (var handler in handlers)
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

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResult>
    {
        var envelope = MessageEnvelope.Wrap(query);
        await RunPublisherPipeline<TQuery>(envelope, ct);

        if (routing.IsExternalMessage(query))
        {
            return await PublishExternallyWithResultAsync<TQuery, TResult>(envelope, ct);
        }

        // handle InMemory
        var (handlerType, handler) = GetHandlerInfo(query);
        return await RunSubscriberPipelineWithResult<TQuery, TResult>(
            envelope,
            handlerType,
            () => HandleMessageInMemoryWithResultAsync<TQuery, TResult>(query, envelope, handlerType, handler, ct),
            ct
        );
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
            IEvent => throw new NotSupportedException($"Can have multiple {nameof(IEventHandler<>)}, use PublishAsync<TEvent> instead"),
            _ => throw new NotSupportedException($"Unsupported message type: {messageType.GetTypeFullName()}"),
        };

        var handlerType = openGenericHandlerType.MakeGenericType(messageType);
        var handler = serviceRegistry.ResolveSingleHandler(handlerType)
            // shouldn't happen as routing said it found InMemory handler, but makes the compiler happy
            ?? throw new ApplicationException($"No in-memory handler found for {messageType.GetTypeFullName()}");

        return (handlerType, handler);
    }

    /// <summary>
    /// Handle the message in memory
    /// </summary>
    private static async Task HandleMessageInMemoryAsync<TMessage>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage
    {
        var handlerName = handlerType.GetTypeFullName();
        try
        {
            await (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, ct])!;
            envelope.MarkSuccess(handlerName);
        }
        catch (Exception ex)
        {
            envelope.MarkFailed(handlerName, ex);
            throw;
        }
    }

    /// <summary>
    /// Handle the message in memory and return the result
    /// </summary>
    private static async Task<TResult> HandleMessageInMemoryWithResultAsync<TMessage, TResult>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage<TResult>
    {
        var handlerName = handlerType.GetTypeFullName();
        try
        {
            var task = (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, ct])!;
            await task.ConfigureAwait(false);
            // Task<TResult> result extraction
            var t = (dynamic)task;
            var result = (TResult)t.GetAwaiter().GetResult();
            envelope.MarkSuccess(handlerName);
            return result;
        }
        catch (Exception ex)
        {
            envelope.MarkFailed(handlerName, ex);
            throw;
        }
    }

    private async Task PublishExternallyAsync<TMessage>(MessageEnvelope envelope, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
    {
        if (externalMessageTransport == null)
        {
            throw new ApplicationException("No external transport is configured and routing strategy requires external transport");
        }

        var options = _defaultExternalTransportOptions.Clone(waitForExecution);

        await RunTransportPipeline<TMessage>(envelope, options, ct);
        var result = await externalMessageTransport.PublishAsync(envelope, options, ct);
        if (result is { Success: true }) return;

        // handler threw an exception - rethrow
        if (result?.Exception != null)
        {
            throw result.Exception;
        }

        var errorMessage = result?.ErrorMessage ?? "External publish message failed";
        if (result == null)
        {
            errorMessage = $"External publish message failed: External transport did not return a result";
            envelope.MarkFailed("External transport", errorMessage);
        }

        throw new ApplicationException(errorMessage);
    }

    private async Task<TResult> PublishExternallyWithResultAsync<TMessage, TResult>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        if (externalMessageTransport == null)
        {
            throw new ApplicationException("No external transport is configured and routing strategy requires external transport");
        }

        // if we want a response, we have to wait for the execution
        var options = _defaultExternalTransportOptions.Clone(waitForExecution: true);

        await RunTransportPipeline<TMessage>(envelope, options, ct);
        var result = await externalMessageTransport!.PublishAsync(envelope, options, ct);
        if (result?.Exception != null) throw result.Exception;

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
            errorMessage = $"Result from transport was marked as a success, but {nameof(ExternalResult.Payload)} was null and {returnType.GetTypeFullName()} is not nullable.";
            envelope.MarkFailed("External transport", errorMessage);
        }

        if (result == null)
        {
            errorMessage = $"External publish message failed: External transport did not return a result";
            envelope.MarkFailed("External transport", errorMessage);
        }

        throw new ApplicationException(errorMessage);
    }

    private Task RunPublisherPipeline<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var ctx = new PublishContext(envelope, serviceRegistry);
        static Task Terminal() => Task.CompletedTask;

        var validMiddlewares = GetValidMiddlewares<IPublisherMiddleware<TMessage>, IPublisherMiddleware<IMessage>>(
            publisherMiddleware);
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));
        return RunPipeline(middlewares, Terminal);
    }

    private Task RunSubscriberPipeline<TMessage>(MessageEnvelope envelope, Type handlerType, Func<Task> terminal, CancellationToken ct)
        where TMessage : IMessage
    {
        envelope.MarkPending(handlerType.GetTypeFullName());
        var ctx = new HandlerContext(envelope, handlerType, serviceRegistry);

        var validMiddlewares = GetValidMiddlewares<ISubscriberMiddleware<TMessage>, ISubscriberMiddleware<IMessage>>(
            subscriberMiddleware);
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        return RunPipeline(middlewares, terminal);
    }

    private async Task<TResult> RunSubscriberPipelineWithResult<TMessage, TResult>(MessageEnvelope envelope, Type handlerType, Func<Task<TResult>> terminal, CancellationToken ct)
        where TMessage : IMessage
    {
        envelope.MarkPending(handlerType.GetTypeFullName());
        var ctx = new HandlerContext(envelope, handlerType, serviceRegistry);

        TResult result = default!;
        async Task NewTerminal() => result = await terminal();

        var validMiddlewares = GetValidMiddlewares<ISubscriberMiddleware<TMessage>, ISubscriberMiddleware<IMessage>>(
            subscriberMiddleware);
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        await RunPipeline(middlewares, NewTerminal);
        return result;
    }

    private Task RunTransportPipeline<TMessage>(MessageEnvelope envelope, IExternalTransportOptions options, CancellationToken ct)
        where TMessage : IMessage
    {
        var ctx = new TransportContext(envelope, options, serviceRegistry);
        static Task Terminal() => Task.CompletedTask;

        var validMiddlewares = GetValidMiddlewares<IExternalTransportPublisherMiddleware<TMessage>, IExternalTransportPublisherMiddleware<IMessage>>(
            externalTransportPublisherMiddleware);
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));

        return RunPipeline(middlewares, Terminal);
    }

    /// <summary>
    /// Get the middlewares that match the give type
    /// </summary>
    private static IEnumerable<TMiddleware> GetValidMiddlewares<TMiddleware, TMiddlewareAll>(
        IEnumerable<TMiddlewareAll>? originalMiddlewares
    )
    {
        var validMiddlewares = new List<TMiddleware>();
        foreach (var mw in originalMiddlewares ?? [])
        {
            if (mw is TMiddleware typed)
            {
                validMiddlewares.Add(typed);
            }
        }
        return validMiddlewares;
    }

    private static Task RunPipeline(IEnumerable<Func<Func<Task>, Func<Task>>>? middlewares, Func<Task> terminal)
    {
        var next = terminal;
        if (middlewares != null)
        {
            foreach (var middleware in middlewares.Reverse())
                next = middleware(next);
        }
        return next();
    }
}
