using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.Transport;

// TODO: retry/error handling policy

public sealed class LocalMessageRouter(
        ILogger<LocalMessageRouter> logger,
        IServiceRegistry serviceRegistry,
        IDefaultProcessorOptions? defaultProcessorOptions,
        IMiddleware[]? subscriberMiddlewares) : IMessageRouter
{
    private readonly IMiddleware[] _middlewares = subscriberMiddlewares ?? [];
    private readonly IDefaultProcessorOptions _defaultProcessorOptions
        = defaultProcessorOptions ?? new DefaultProcessorOptions();

    /// <summary>
    /// Locally, there is no difference between fire-and-forget Send and immediate Execute for commands
    /// </summary>
    public Task SendAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct) where TCommand : ICommand
        => ExecuteAsync<TCommand>(envelope, ct);

    public async Task ExecuteAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand
    {
        var handler = serviceRegistry.ResolveRequiredService<ICommandHandler<TCommand>>();
        var handlerType = handler.GetType();
        using (logger.BeginScope(envelope.BuildLoggingScopeData(handlerType)))
        {
            await RunSubscriberPipeline<TCommand>(
                envelope,
                handlerType,
                handlerFunc: async message => await handler.HandleAsync(message),
                ct
            );
        }
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(
        MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand<TResponse>
    {
        var handler = serviceRegistry.ResolveRequiredService<ICommandHandler<TCommand, TResponse>>();
        var handlerType = handler.GetType();
        using (logger.BeginScope(envelope.BuildLoggingScopeData(handlerType)))
        {
            return await RunSubscriberPipelineWithResponse<TCommand, TResponse>(
                envelope,
                handlerType,
                handlerFunc: message => handler.HandleAsync(message),
                ct
            );
        }
    }

    public async Task PublishManyAsync<TEvent>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TEvent : IEvent
    {
        if (envelopes.Length == 0) return;
        var prioritisedHandlers = serviceRegistry.ResolveServices<IEventHandler<TEvent>>()
                                    // TODO: a way of prioritising event handlers
                                    // default priority = 0
                                    .GroupBy(_ => 0)
                                    .OrderBy(x => x.Key) // return handler groups in priority order
                                    .ToList();
        foreach (var groupedHandlers in prioritisedHandlers)
        {
            foreach (var handler in groupedHandlers.ToList())
            {
                var handlerType = handler.GetType();
                foreach (var envelope in envelopes)
                {
                    using (logger.BeginScope(envelope.BuildLoggingScopeData(handlerType)))
                    {
                        await RunSubscriberPipeline<TEvent>(
                            envelope,
                            handlerType,
                            handlerFunc: async message => await handler.HandleAsync(message),
                            ct
                        );
                    }
                }
            }
        }
    }

    public async Task<TResponse> QueryAsync<TQuery, TResponse>(
        MessageEnvelope envelope, CancellationToken ct)
        where TQuery : IQuery<TResponse>
    {
        var handler = serviceRegistry.ResolveRequiredService<IQueryHandler<TQuery, TResponse>>();
        var handlerType = handler.GetType();

        using (logger.BeginScope(envelope.BuildLoggingScopeData(handlerType)))
        {
            return await RunSubscriberPipelineWithResponse<TQuery, TResponse>(
                envelope,
                handlerType,
                handlerFunc: message => handler.HandleAsync(message),
                ct
            );
        }
    }

    //private async Task<TResponse> RequestResponseAsync<TRequest, TResponse, THandler>(
    //    MessageEnvelope envelope, CancellationToken ct)
    //    where TRequest : IMessage<TResponse>
    //    where THandler : IRequestHandler<TRequest, TResponse>
    //{
    //    var handler = serviceRegistry.ResolveRequiredService<THandler>();
    //    var handlerType = handler.GetType();

    //    using (logger.BeginScope(envelope.BuildLoggingScopeData(handlerType)))
    //    {
    //        return await RunSubscriberPipelineWithResponse<TRequest, TResponse>(
    //            envelope,
    //            handlerType,
    //            handlerFunc: message => handler.HandleAsync(message),
    //            ct
    //        );
    //    }
    //}

    private Task RunSubscriberPipeline<TMessage>(
        MessageEnvelope envelope, Type handlerType, Func<TMessage, Task> handlerFunc, CancellationToken ct)
        where TMessage : IMessage
    {
        var terminal = () => HandleMessageLocallyAsync(envelope, handlerType, handlerFunc, ct);
        envelope.MarkPending(handlerType.GetTypeFullName());
        return _middlewares.InvokeAsync<TMessage>(envelope, terminal, ct);
    }

    private async Task<TResponse> RunSubscriberPipelineWithResponse<TMessage, TResponse>(
            MessageEnvelope envelope,
            Type handlerType,
            Func<TMessage, Task<TResponse>> handlerFunc,
            CancellationToken ct
        ) where TMessage : IMessage<TResponse>
    {
        envelope.MarkPending(handlerType.GetTypeFullName());

        TResponse response = default!;
        var terminal = async () =>
        {
            response = await HandleMessageLocallyWithResponseAsync(
                envelope,
                handlerType,
                handlerFunc,
                ct
            );
        };
        await _middlewares.InvokeAsync<TMessage>(envelope, terminal, ct);
        return response;
    }

    /// <summary>
    /// Handle the message locally
    /// </summary>
    private async Task HandleMessageLocallyAsync<TMessage>(
        MessageEnvelope envelope,
        Type handlerType,
        Func<TMessage, Task> handlerFunc,
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
            await handlerFunc.Invoke((TMessage)envelope.Message);
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
    /// Handle the message locally and return the response
    /// </summary>
    private async Task<TResponse> HandleMessageLocallyWithResponseAsync<TMessage, TResponse>(
        MessageEnvelope envelope,
        Type handlerType,
        Func<TMessage, Task<TResponse>> handlerFunc,
        CancellationToken ct) where TMessage : IMessage<TResponse>
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
            var response = await handlerFunc.Invoke((TMessage)envelope.Message);
            envelope.MarkSuccess(handlerTypeName);
            logger.LogDebug(
            "Stop: Handled the {MessageType} by {HandlerType} successfully", messageTypeName, handlerTypeName);
            return response;
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
