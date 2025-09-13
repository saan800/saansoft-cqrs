
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus.External;
using SaanSoft.Cqrs.Bus.InMemory;
using SaanSoft.Cqrs.Bus.Utilities;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public sealed class MessageBus(
    ILogger<MessageBus> logger,
    IServiceRegistry serviceRegistry,
    IRoutingStrategy routing,
    IInMemoryMessageProcessor inMemoryMessageProcessor,
    IEnumerable<IPublisherMiddleware<IMessage>>? publisherMiddleware,
    IExternalMessageProcessor? externalMessageProcessor) : IMessageBus
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            var envelope = MessageEnvelope.Wrap(command);
            await RunPublisherPipeline<TCommand>(envelope, ct);

            if (routing.IsExternalMessage(command) && externalMessageProcessor != null)
            {
                await externalMessageProcessor.PublishExternallyAndWaitAsync<TCommand>(envelope, ct);
                return;
            }

            await inMemoryMessageProcessor.HandleCommandEnvelopeAsync<TCommand>(envelope, ct);
        }
    }

    public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResult>
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            var envelope = MessageEnvelope.Wrap(command);
            await RunPublisherPipeline<TCommand>(envelope, ct);

            return routing.IsExternalMessage(command) && externalMessageProcessor != null
                ? await externalMessageProcessor.PublishExternallyAndWaitForResultsAsync<TCommand, TResult>(envelope, ct)
                : await inMemoryMessageProcessor.HandleCommandEnvelopeAsync<TCommand, TResult>(envelope, ct);
        }
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            var envelope = MessageEnvelope.Wrap(command);
            await RunPublisherPipeline<TCommand>(envelope, ct);

            if (routing.IsExternalMessage(command) && externalMessageProcessor != null)
            {
                await externalMessageProcessor.PublishExternallyAsync<TCommand>([envelope], ct);
                return;
            }

            await inMemoryMessageProcessor.HandleCommandEnvelopeAsync<TCommand>(envelope, ct);
        }
    }

    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent
    {
        using (logger.BeginScope(evt.BuildLoggingScopeData()))
        {
            await PublishManyAsync([evt], ct);
        }
    }

    public async Task PublishManyAsync<TEvent>(IReadOnlyCollection<TEvent> events, CancellationToken ct = default)
        where TEvent : IEvent
    {
        if (events.Count == 0) return;
        var firstEvent = events.First();

        var envelopeTasks = events.Select(async evt =>
        {
            var envelope = MessageEnvelope.Wrap(evt);
            await RunPublisherPipeline<TEvent>(envelope, ct);
            return envelope;
        });
        var envelopes = await Task.WhenAll(envelopeTasks);

        if (routing.IsExternalMessage(firstEvent) && externalMessageProcessor != null)
        {
            await externalMessageProcessor.PublishExternallyAsync<TEvent>(envelopes, ct);
            return;
        }

        await inMemoryMessageProcessor.HandleEventEnvelopesAsync<TEvent>(envelopes, ct);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResult>
    {
        using (logger.BeginScope(query.BuildLoggingScopeData()))
        {
            var envelope = MessageEnvelope.Wrap(query);
            await RunPublisherPipeline<TQuery>(envelope, ct);

            return routing.IsExternalMessage(query) && externalMessageProcessor != null
                ? await externalMessageProcessor.PublishExternallyAndWaitForResultsAsync<TQuery, TResult>(envelope, ct)
                : await inMemoryMessageProcessor.HandleQueryEnvelopeAsync<TQuery, TResult>(envelope, ct);
        }
    }

    private Task RunPublisherPipeline<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
    {
        var ctx = new PublishContext(envelope, serviceRegistry);
        static Task Terminal() => Task.CompletedTask;

        var validMiddlewares = publisherMiddleware
            .GetValidMiddlewares<IPublisherMiddleware<TMessage>, IPublisherMiddleware<IMessage>>();
        var middlewares = validMiddlewares
            .Select(mw => (Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync(ctx, next, ct)));
        return MiddlewareExtensions.RunPipeline(middlewares, Terminal);
    }
}
