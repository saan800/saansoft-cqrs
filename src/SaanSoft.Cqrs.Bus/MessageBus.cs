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
    IReadOnlyCollection<IMiddleware>? publisherMiddlewares = null) : IMessageBus
{
    private readonly IMiddleware[] _middlewares = publisherMiddlewares?.ToArray() ?? [];

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

    public Task<TResponse> ExecuteAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand<TResponse>
        => ExecuteMessageWithResponseAsync<TCommand, TResponse>(command, ct, callerFile);

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

    public Task SendManyAsync<TCommand>(
        IReadOnlyCollection<TCommand> commands,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand
        => SendManyMessagesAsync(commands, ct, callerFile);


    public Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TQuery : IQuery<TResponse>
        => ExecuteMessageWithResponseAsync<TQuery, TResponse>(query, ct, callerFile);

    public Task PublishAsync<TEvent>(
        TEvent evt,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent
        => SendManyMessagesAsync([evt], ct, callerFile);

    public Task PublishManyAsync<TEvent>(
        IReadOnlyCollection<TEvent> events,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent
        => SendManyMessagesAsync(events, ct, callerFile);

    private async Task<TResponse> ExecuteMessageWithResponseAsync<TMessage, TResponse>(
        TMessage message,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TMessage : IMessage<TResponse>
    {
        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelope = MessageEnvelope.Wrap(message, callerClass);

        using (logger.BeginScope(envelope.BuildLoggingScopeData()))
        {
            await _middlewares.InvokeAsync<TMessage>(envelope, ct);

            var messageRouter = routing.GetMessageRouter<TMessage>();
            return await messageRouter.ExecuteAsync<TMessage, TResponse>(envelope, ct);
        }
    }

    private async Task SendManyMessagesAsync<TMessage>(
        IReadOnlyCollection<TMessage> messages,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TMessage : IMessage
    {
        if (messages.Count == 0) return;

        var callerClass = Path.GetFileNameWithoutExtension(callerFile);
        var envelopes = messages.Select(evt => MessageEnvelope.Wrap(evt, callerClass)).ToArray();
        using (logger.BeginScope(envelopes.BuildLoggingScopeData()))
        {
            var tasks = envelopes.Select(async envelope => await _middlewares.InvokeAsync<TMessage>(envelope, ct));
            await Task.WhenAll(tasks);

            var messageRouter = routing.GetMessageRouter<TMessage>();
            await messageRouter.SendManyAsync<TMessage>(envelopes, ct);
        }
    }
}
