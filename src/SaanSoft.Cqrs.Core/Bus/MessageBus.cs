
using SaanSoft.Cqrs.Core.Transport;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Core.Bus;

public sealed class MessageBus(
    IServiceRegistry serviceRegistry,
    IRoutingStrategy routing,
    IDefaultExternalTransportOptions? defaultExternalTransportOptions,
    IEnumerable<IPublisherMiddleware<IMessage>>? publisherMiddleware,
    IEnumerable<ISubscriberMiddleware<IMessage>>? subscriberMiddleware,
    IExternalMessageTransport? externalMessageTransport,
    IEnumerable<IExternalTransportPublisherMiddleware<IMessage>>? externalTransportPublisherMiddleware) : IMessageBus
{
    private readonly string _externalMessageTransportName = "External transport";
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

    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent
        => PublishManyAsync([evt], ct);

    public async Task PublishManyAsync<TEvent>(IReadOnlyCollection<TEvent> events, CancellationToken ct = default)
        where TEvent : IEvent
    {
        if (events.Count == 0) return;

        var envelopeTasks = events.Select(async evt =>
        {
            var envelope = MessageEnvelope.Wrap(evt);
            await RunPublisherPipeline<TEvent>(envelope, ct);
            return envelope;
        });
        var envelopes = await Task.WhenAll(envelopeTasks);
        var firstEvent = events.First();

        if (routing.IsExternalMessage(firstEvent))
        {
            await PublishExternallyAsync<TEvent>(envelopes, waitForExecution: false, ct);
            return;
        }

        // handle InMemory
        var handlerType = typeof(IEventHandler<>).MakeGenericType(firstEvent.GetType());
        var handlers = serviceRegistry.ResolveMultipleHandlers(handlerType).ToArray();
        foreach (var handler in handlers)
        {
            foreach (var envelope in envelopes)
            {
                var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
                envelope.MarkPending(handlerName);

                await RunSubscriberPipeline<TEvent>(
                    envelope,
                    handlerType,
                    () => HandleMessageInMemoryAsync((IEvent)envelope.Message, envelope, handlerType, handler, ct),
                    ct
                );
            }
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
            IEvent => throw new NotSupportedException($"Can have multiple IEventHandler<>, use PublishAsync<TEvent> instead"),
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
    private async Task HandleMessageInMemoryAsync<TMessage>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage
    {
        var timeout = _defaultExternalTransportOptions.Timeout;
        if (envelope.Message is ITimeout timeoutMessage)
            timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var handlerName = handlerType.GetTypeFullName();
        try
        {
            await (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, linkedCts.Token])!;
            envelope.MarkSuccess(handlerName);
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException($"Handler {handlerName} timed out after {timeout}.", ex);
            envelope.MarkFailed(handlerName, exception);
            throw exception;
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
    private async Task<TResult> HandleMessageInMemoryWithResultAsync<TMessage, TResult>(
        TMessage message,
        MessageEnvelope envelope,
        Type handlerType,
        object handler,
        CancellationToken ct) where TMessage : IMessage<TResult>
    {
        var timeout = _defaultExternalTransportOptions.Timeout;
        if (envelope.Message is ITimeout timeoutMessage)
            timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        var handlerName = handlerType.GetTypeFullName();
        try
        {
            var task = (Task)handlerType.GetMethod("HandleAsync")!
                .Invoke(handler, [message, linkedCts.Token])!;
            await task.ConfigureAwait(false);
            // Task<TResult> result extraction
            var t = (dynamic)task;
            var result = (TResult)t.GetAwaiter().GetResult();
            envelope.MarkSuccess(handlerName);
            return result;
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException($"Handler {handlerName} timed out after {timeout}.", ex);
            envelope.MarkFailed(handlerName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            envelope.MarkFailed(handlerName, ex);
            throw;
        }
    }

    private Task PublishExternallyAsync<TMessage>(MessageEnvelope envelope, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
        => PublishExternallyAsync<TMessage>([envelope], waitForExecution, ct);

    private async Task PublishExternallyAsync<TMessage>(MessageEnvelope[] envelopes, bool waitForExecution, CancellationToken ct)
        where TMessage : IMessage
    {
        if (externalMessageTransport == null)
        {
            throw new ApplicationException("No external transport is configured and routing strategy requires external transport");
        }
        if (envelopes.Length == 0) return;
        if (waitForExecution && envelopes.Length > 1)
        {
            // this should never happen, as this is a private method so we should ensure this is not a thing.
            throw new ApplicationException("Cannot wait for multiple messages to execute. Only messages that are fire-and-forget can publish multiple messages at once.");
        }

        var firstMessage = envelopes.First().Message;

        var options = _defaultExternalTransportOptions.Clone(waitForExecution);
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

            var result = await externalMessageTransport.PublishManyAsync(envelopes, options, transportCt);
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
            }

            throw new ApplicationException(errorMessage);
        }
        catch (OperationCanceledException ex) when (timeoutCts.IsCancellationRequested)
        {
            var exception = new TimeoutException($"External publish message(s) failed: Timed out after {options.Timeout}.", ex);

            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageTransportName, exception);
            }
            throw exception;
        }
        catch (Exception ex)
        {
            foreach (var envelope in envelopes)
            {
                envelope.MarkFailed(_externalMessageTransportName, ex);
            }
            throw;
        }
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
        options.ExpectSingleHandler = envelope.Message is not IEvent;
        if (envelope.Message is ITimeout timeoutMessage)
            options.Timeout = timeoutMessage.Timeout;

        using var timeoutCts = new CancellationTokenSource(options.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var transportCt = linkedCts.Token;

            await RunTransportPipeline<TMessage>(envelope, transportCt);
            var result = await externalMessageTransport!.PublishAsync(envelope, options, transportCt);
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
            var exception = new TimeoutException($"External publish message failed: Timed out after {options.Timeout}.", ex);
            envelope.MarkFailed(_externalMessageTransportName, exception);
            throw exception;
        }
        catch (Exception ex)
        {
            envelope.MarkFailed(_externalMessageTransportName, ex);
            throw;
        }
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

    private Task RunTransportPipeline<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var ctx = new TransportContext(envelope, serviceRegistry);
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
