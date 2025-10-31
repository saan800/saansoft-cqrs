using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.Transport;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public sealed class MessageBus(
    ILogger<MessageBus> logger,
    RoutingStrategy routing,
    IMiddleware[]? publisherMiddlewares) : IMessageBus
{
    private readonly IMiddleware[] _middlewares = publisherMiddlewares ?? [];

    public async Task ExecuteAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(command, callerClass);

        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TCommand>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TCommand>();
            await messageRouter.ExecuteAsync<TCommand>(envelope, ct);
        }
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand<TResponse>
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(command, callerClass);

        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TCommand>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TCommand>();
            return await messageRouter.ExecuteAsync<TCommand, TResponse>(envelope, ct);
        }
    }

    public async Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(command, callerClass);

        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TCommand>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TCommand>();
            await messageRouter.SendAsync<TCommand>(envelope, ct);
        }
    }

    public async Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TQuery : IQuery<TResponse>
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(query, callerClass);

        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TQuery>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TQuery>();
            return await messageRouter.QueryAsync<TQuery, TResponse>(envelope, ct);
        }
    }

    public async Task PublishAsync<TEvent>(
        TEvent evt,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(evt, callerClass);
        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TEvent>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TEvent>();
            await messageRouter.PublishManyAsync<TEvent>([envelope], ct);
        }
    }

    public async Task PublishManyAsync<TEvent>(
        IReadOnlyCollection<TEvent> events,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent
    {
        if (events.Count == 0) return;

        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelopes = events.Select(evt => MessageEnvelope.Wrap(evt, callerClass)).ToArray();
        using (logger.BeginScope(envelopes.BuildLoggingScopeData()))
        {
            var tasks = envelopes.Select(async envelope => await _middlewares.InvokeAsync<TEvent>(envelope, ct));
            await Task.WhenAll(tasks);

            var messageRouter = routing.GetMessageRouter<TEvent>();
            await messageRouter.PublishManyAsync<TEvent>(envelopes, ct);
        }
    }
}
