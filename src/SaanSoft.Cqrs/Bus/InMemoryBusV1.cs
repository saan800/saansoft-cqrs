using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Cqrs.Bus;


// TODO: v1, still needed?

// public sealed class InMemoryBus(
//     IServiceProvider serviceProvider,
//     IEnumerable<object> publishMiddlewares,
//     IExternalPublisher? externalPublisher
// ) : IInMemoryBus
// {
//     private readonly IEnumerable<object> _publishMiddlewares = publishMiddlewares; // open generics are tough, we filter at runtime
//     private readonly IExternalPublisher? _externalPublisher = externalPublisher;

//     public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
//     => InvokePipeline(command, async (msg, cancellationToken) =>
//     {
//         var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
//         await handler.HandleAsync(msg, cancellationToken);
//     }, ct);

//     public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
//         where TCommand : ICommand<TResult>
//     {
//         // Run publish-side middlewares first (enrichment, add headers, correlation, etc.)
//         await RunPublishMiddlewares(command, ct);

//         var handlers = serviceProvider.GetServices<ICommandHandler<TCommand, TResult>>().ToArray();

//         if (handlers.Length > 1)
//         {
//             throw new InvalidOperationException($"Multiple handlers found for command {typeof(TCommand).FullName}");
//         }
//         // handled locally
//         if (handlers.Length == 1)
//         {
//             // single internal handler — run subscriber-side middlewares and return result
//             var handler = handlers[0];
//             // run subscriber middlewares pipeline (so decorators are applied)
//             TResult result = await RunSubscriberPipelineAndInvoke(handler, command, ct);
//             return result;
//         }

//         // else: no internal handler or multiple handlers -> publish externally
//         var envelope = MessageEnvelope.Wrap(command, publisher: "app://local");
//         // store log + send to external publisher
//         await _externalPublisher.PublishAsync(envelope, ct);

//         // Wait for the response — this requires a correlation-based reply mechanism.
//         // Implementation choices:
//         // - synchronous waiting on a TaskCompletionSource keyed by CorrelationId (if your environment supports it)
//         // - or use a reply-to queue and await an external response message
//         return await WaitForExternalResponse<TResult>(command.CorrelationId, ct);
//     }


//     public Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : IEvent
//     => InvokePipeline(evt, async (msg, cancellationToken) =>
//     {
//         var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
//         foreach (var h in handlers)
//             await h.HandleAsync(msg, cancellationToken);
//     }, ct);


//     public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResult>
//     => InvokePipeline(query, async (msg, cancellationToken) =>
//     {
//         var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
//         return await handler.HandleAsync(msg, cancellationToken);
//     }, ct);


//     private async Task InvokePipeline<TMessage>(TMessage message, Func<TMessage, CancellationToken, Task> terminal, CancellationToken ct)
//         where TMessage : IMessage
//     {
//         var chain = BuildPipeline(message, terminal, ct);
//         await chain(message, ct);
//     }


//     private async Task<TResult> InvokePipeline<TMessage, TResult>(TMessage message, Func<TMessage, CancellationToken, Task<TResult>> terminal, CancellationToken ct)
//         where TMessage : IMessage
//     {
//         TResult? response = default!;
//         var chain = BuildPipeline(message, async (msg, cancellationToken) =>
//         {
//             response = await terminal(msg, cancellationToken);
//         }, ct);
//         await chain(message, ct);
//         return response!;
//     }


//     /// <summary>
//     /// This chains together all applicable middlewares to enrich the message before invoking the actual handler.
//     /// It filters the middlewares at runtime based on whether they implement IPublishMiddleware{TMessage}.
//     /// </summary>
//     private async Task<TMessage> RunPublishMiddlewares<TMessage>(TMessage message, CancellationToken ct)
//         where TMessage : IMessage
//     {
//         foreach (var mw in _publishMiddlewares)
//         {
//             if (mw is IPublishMiddleware<TMessage> typed)
//             {
//                 message = await typed.InvokeAsync(message, async (msg, cancellationToken) =>
//                 {
//                     // terminal - just return the message
//                     return await Task.FromResult(msg);
//                 }, ct);
//             }
//         }
//         return message;
//     }


//     private PublishHandlerDelegate<TMessage> BuildPipeline<TMessage>(TMessage message, Func<TMessage, CancellationToken, Task> terminal, CancellationToken ct)
//         where TMessage : IMessage
//     {
//         PublishHandlerDelegate<TMessage> next = (msg, cancellationToken) => (Task<TMessage>)terminal(msg, cancellationToken);

//         // build the chain in reverse order, so the first middleware in the list is the outermost
//         // and the last middleware wraps the terminal handler
//         // this allows middleware ordering to be controlled by registration order
//         foreach (var mw in _publishMiddlewares)
//         {
//             // runtime check for open generic interface implementation, so could be for all IMessages,
//             // or just for ICommand, IEvent, IQuery<T>, or even specific message types
//             if (mw is IPublishMiddleware<TMessage> typed)
//             {
//                 var currentNext = next;
//                 next = (msg, cancellationToken) => typed.InvokeAsync(msg, currentNext, cancellationToken);
//             }
//         }

//         return next;
//     }
// }
