// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using SaanSoft.Cqrs.Middleware;

// namespace CqrsLite
// {
//     // ---------------------------
//     // 1) Message primitives
//     // ---------------------------

//     public interface IMessage
//     {
//         Guid Id { get; }
//         DateTime MessageOn { get; }
//     }

//     public interface ICommand : IMessage { }
//     public interface ICommand<TResult> : IMessage { }
//     public interface IQuery<TResult> : IMessage { }
//     public interface IEvent : IMessage { }

//     public abstract record MessageBase(Guid Id, DateTime MessageOn) : IMessage
//     {
//         protected MessageBase() : this(Guid.NewGuid(), DateTime.UtcNow) { }
//     }

//     // ---------------------------
//     // 2) Envelope + handler tracking
//     // ---------------------------

//     public enum HandlerStatus { Pending, Success, Failed }

//     public sealed record HandlerRecord(
//         string HandlerName,
//         DateTime? HandledOnUtc,
//         HandlerStatus Status,
//         string? Error
//     );

//     /// <summary>
//     /// Non-generic envelope to make transports simpler to implement.
//     /// </summary>
//     public sealed class MessageEnvelope
//     {
//         public Guid Id { get; init; }
//         public DateTime MessageOn { get; init; }
//         public string MessageType { get; init; }
//         public string? Publisher { get; set; }
//         public List<HandlerRecord> Handlers { get; } = new();
//         public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);
//         public object OriginalMessage { get; init; }

//         public MessageEnvelope(object message, string? publisher = null)
//         {
//             if (message is not IMessage m) throw new ArgumentException("Original must implement IMessage");
//             Id = m.Id;
//             MessageOn = m.MessageOn;
//             MessageType = message.GetType().FullName ?? message.GetType().Name;
//             OriginalMessage = message;
//             Publisher = publisher;
//         }

//         public void MarkPending(string handlerName)
//             => Handlers.Add(new HandlerRecord(handlerName, null, HandlerStatus.Pending, null));

//         public void MarkSuccess(string handlerName)
//         {
//             var idx = Handlers.FindIndex(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Pending);
//             if (idx >= 0)
//                 Handlers[idx] = Handlers[idx] with { HandledOnUtc = DateTime.UtcNow, Status = HandlerStatus.Success };
//             else
//                 Handlers.Add(new HandlerRecord(handlerName, DateTime.UtcNow, HandlerStatus.Success, null));
//         }

//         public void MarkFailed(string handlerName, string error)
//         {
//             var idx = Handlers.FindIndex(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Pending);
//             if (idx >= 0)
//                 Handlers[idx] = Handlers[idx] with { HandledOnUtc = DateTime.UtcNow, Status = HandlerStatus.Failed, Error = error };
//             else
//                 Handlers.Add(new HandlerRecord(handlerName, DateTime.UtcNow, HandlerStatus.Failed, error));
//         }
//     }

//     // ---------------------------
//     // 3) Handler contracts
//     // ---------------------------

//     public interface ICommandHandler<TCommand> where TCommand : ICommand
//     {
//         Task HandleAsync(TCommand command, MessageEnvelope envelope, CancellationToken ct);
//     }

//     public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
//     {
//         Task<TResult> HandleAsync(TCommand command, MessageEnvelope envelope, CancellationToken ct);
//     }

//     public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
//     {
//         Task<TResult> HandleAsync(TQuery query, MessageEnvelope envelope, CancellationToken ct);
//     }

//     public interface IEventHandler<TEvent> where TEvent : IEvent
//     {
//         Task HandleAsync(TEvent @event, MessageEnvelope envelope, CancellationToken ct);
//     }

//     // ---------------------------
//     // 4) Buses
//     // ---------------------------

//     public interface ICommandBus
//     {
//         Task SendAsync(ICommand command, CancellationToken ct = default);
//         Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default);
//     }

//     public interface IQueryBus
//     {
//         Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
//     }

//     public interface IEventBus
//     {
//         Task PublishAsync(IEvent @event, CancellationToken ct = default);
//     }

//     // ---------------------------
//     // 5) Publisher + Subscriber pipelines (decorators)
//     // ---------------------------

//     public interface IPublisherFilter
//     {
//         Task InvokeAsync(PublishContext context, Func<Task> next);
//     }

//     public interface ISubscriberFilter
//     {
//         Task InvokeAsync(HandleContext context, Func<Task> next);
//     }

//     public sealed class PublishContext
//     {
//         public MessageEnvelope Envelope { get; }
//         public IServiceProvider Services { get; }
//         public CancellationToken CancellationToken { get; }
//         public PublishContext(MessageEnvelope envelope, IServiceProvider services, CancellationToken ct)
//         {
//             Envelope = envelope; Services = services; CancellationToken = ct;
//         }
//     }

//     public sealed class HandleContext
//     {
//         public MessageEnvelope Envelope { get; }
//         public string HandlerName { get; }
//         public IServiceProvider Services { get; }
//         public CancellationToken CancellationToken { get; }
//         public HandleContext(MessageEnvelope envelope, string handlerName, IServiceProvider services, CancellationToken ct)
//         {
//             Envelope = envelope; HandlerName = handlerName; Services = services; CancellationToken = ct;
//         }
//     }

//     // ---------------------------
//     // 6) External transport (stub) + filters
//     // ---------------------------

//     public sealed class TransportOptions
//     {
//         public TimeSpan? Timeout { get; init; }
//         public Dictionary<string, string> Metadata { get; } = new();
//     }

//     public sealed class ExternalResult
//     {
//         public bool Success { get; init; }
//         public object? Payload { get; init; } // TResult for queries/commands with results
//         public string? Error { get; init; }
//     }

//     /// <summary>
//     /// Interface for an external pub/sub provider (e.g., Azure Service Bus).
//     /// This is intentionally a stub. Implementers perform serialization and I/O.
//     /// </summary>
//     public interface IExternalMessageTransport
//     {
//         Task<ExternalResult?> SendAsync(MessageEnvelope envelope, TransportOptions? options, CancellationToken ct);
//     }

//     public interface ITransportFilter
//     {
//         Task InvokeAsync(TransportContext context, Func<Task> next);
//     }

//     public sealed class TransportContext
//     {
//         public MessageEnvelope Envelope { get; }
//         public TransportOptions Options { get; }
//         public CancellationToken CancellationToken { get; }
//         public TransportContext(MessageEnvelope envelope, TransportOptions options, CancellationToken ct)
//         {
//             Envelope = envelope; Options = options; CancellationToken = ct;
//         }
//     }

//     // ---------------------------
//     // 7) Routing strategy
//     // ---------------------------

//     public sealed record RouteDecision(bool UseExternal, string? Reason = null);

//     public interface IRoutingStrategy
//     {
//         RouteDecision Decide(object message);
//     }

//     /// <summary>
//     /// Default: use in-memory if a handler is registered; otherwise external.
//     /// </summary>
//     public sealed class DefaultRoutingStrategy : IRoutingStrategy
//     {
//         private readonly IHandlerRegistry _registry;
//         public DefaultRoutingStrategy(IHandlerRegistry registry) => _registry = registry;

//         public RouteDecision Decide(object message)
//         {
//             var t = message.GetType();
//             if (Implements(t, typeof(IEvent))) // events often go external by default but we keep it deterministic
//             {
//                 var any = _registry.HasEventHandlers(t);
//                 return any ? new(false, "In-memory event handlers found") : new(true, "No in-memory event handlers");
//             }
//             if (Closes(t, typeof(ICommand<>)))
//             {
//                 var any = _registry.HasCommandResultHandler(t);
//                 return any ? new(false, "In-memory command<TResult> handler found") : new(true, "No in-memory handler");
//             }
//             if (Implements(t, typeof(ICommand)))
//             {
//                 var any = _registry.HasCommandHandler(t);
//                 return any ? new(false, "In-memory command handler found") : new(true, "No in-memory handler");
//             }
//             if (Closes(t, typeof(IQuery<>)))
//             {
//                 var any = _registry.HasQueryHandler(t);
//                 return any ? new(false, "In-memory query handler found") : new(true, "No in-memory handler");
//             }
//             return new(true, "Unknown message type");
//         }

//         private static bool Implements(Type t, Type iface) => iface.IsAssignableFrom(t);
//         private static bool Closes(Type t, Type openGeneric) =>
//             t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
//     }

//     // ---------------------------
//     // 8) Handler registry (DI-friendly)
//     // ---------------------------

//     public interface IHandlerRegistry
//     {
//         bool HasCommandHandler(Type commandType);
//         bool HasCommandResultHandler(Type commandType); // ICommand<TResult>
//         bool HasQueryHandler(Type queryType);           // IQuery<TResult>
//         bool HasEventHandlers(Type eventType);

//         object? ResolveSingle(Type handlerType);
//         IEnumerable<object> ResolveMultiple(Type handlerType);
//     }

//     /// <summary>
//     /// Minimal registry backed by IServiceProvider.
//     /// </summary>
//     public sealed class ServiceProviderHandlerRegistry : IHandlerRegistry
//     {
//         private readonly IServiceProvider _sp;

//         public ServiceProviderHandlerRegistry(IServiceProvider sp) => _sp = sp;

//         public bool HasCommandHandler(Type commandType)
//             => ResolveSingle(typeof(ICommandHandler<>).MakeGenericType(commandType)) != null;

//         public bool HasCommandResultHandler(Type commandType) // T : ICommand<TResult>
//         {
//             var i = commandType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommand<>));
//             if (i == null) return false;
//             var tResult = i.GetGenericArguments()[0];
//             var hType = typeof(ICommandHandler<,>).MakeGenericType(commandType, tResult);
//             return ResolveSingle(hType) != null;
//         }

//         public bool HasQueryHandler(Type queryType)
//         {
//             var i = queryType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuery<>));
//             if (i == null) return false;
//             var tResult = i.GetGenericArguments()[0];
//             var hType = typeof(IQueryHandler<,>).MakeGenericType(queryType, tResult);
//             return ResolveSingle(hType) != null;
//         }

//         public bool HasEventHandlers(Type eventType)
//             => ResolveMultiple(typeof(IEventHandler<>).MakeGenericType(eventType)).Any();

//         public object? ResolveSingle(Type handlerType)
//         {
//             // Typical DI containers expose GetService/ GetRequiredService / IEnumerable<>
//             return _sp.GetService(handlerType);
//         }

//         public IEnumerable<object> ResolveMultiple(Type handlerType)
//         {
//             var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);
//             var resolved = _sp.GetService(enumerableType) as System.Collections.IEnumerable;
//             if (resolved == null) yield break;
//             foreach (var item in resolved) yield return item!;
//         }
//     }

//     // ---------------------------
//     // 9) Pipeline helpers
//     // ---------------------------

//     internal static class Pipeline
//     {
//         public static Task Run(IEnumerable<Func<Func<Task>, Func<Task>>> middlewares, Func<Task> terminal)
//         {
//             var next = terminal;
//             foreach (var middleware in middlewares.Reverse())
//                 next = middleware(next);
//             return next();
//         }
//     }

//     // ---------------------------
//     // 10) Bus implementations (hide routing)
//     // ---------------------------

//     public sealed class CommandBus : ICommandBus
//     {
//         private readonly IHandlerRegistry _registry;
//         private readonly IRoutingStrategy _routing;
//         private readonly IEnumerable<IPublisherFilter> _publisherFilters;
//         private readonly IEnumerable<ISubscriberFilter> _subscriberFilters;
//         private readonly IExternalMessageTransport _transport;
//         private readonly IEnumerable<ITransportFilter> _transportFilters;
//         private readonly IServiceProvider _services;

//         public CommandBus(
//             IHandlerRegistry registry,
//             IRoutingStrategy routing,
//             IEnumerable<IPublisherFilter> publisherFilters,
//             IEnumerable<ISubscriberFilter> subscriberFilters,
//             IExternalMessageTransport transport,
//             IEnumerable<ITransportFilter> transportFilters,
//             IServiceProvider services)
//         {
//             _registry = registry;
//             _routing = routing;
//             _publisherFilters = publisherFilters;
//             _subscriberFilters = subscriberFilters;
//             _transport = transport;
//             _transportFilters = transportFilters;
//             _services = services;
//         }

//         public async Task SendAsync(ICommand command, CancellationToken ct = default)
//         {
//             var envelope = new MessageEnvelope(command);
//             await RunPublisherPipeline(envelope, ct);

//             var decision = _routing.Decide(command);

//             if (!decision.UseExternal)
//             {
//                 var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
//                 var handler = _registry.ResolveSingle(handlerType)
//                     ?? throw new InvalidOperationException($"No in-memory handler found for {command.GetType().Name}");

//                 var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
//                 envelope.MarkPending(handlerName);

//                 var handleCtx = new HandleContext(envelope, handlerName, _services, ct);
//                 await Pipeline.Run(
//                     _subscriberFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(handleCtx, next))),
//                     async () =>
//                     {
//                         try
//                         {
//                             var task = (Task)handlerType.GetMethod("HandleAsync")!
//                                 .Invoke(handler, new object[] { command, envelope, ct })!;
//                             await task.ConfigureAwait(false);
//                             envelope.MarkSuccess(handlerName);
//                         }
//                         catch (Exception ex)
//                         {
//                             envelope.MarkFailed(handlerName, ex.ToString());
//                             throw;
//                         }
//                     });
//             }
//             else
//             {
//                 var options = new TransportOptions();
//                 await RunTransportPipeline(envelope, options, ct);
//                 var result = await _transport.SendAsync(envelope, options, ct).ConfigureAwait(false);
//                 if (result is { Success: false })
//                     throw new InvalidOperationException(result.Error ?? "External send failed");
//             }
//         }

//         public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default)
//         {
//             var envelope = new MessageEnvelope(command);
//             await RunPublisherPipeline(envelope, ct);

//             var decision = _routing.Decide(command);

//             if (!decision.UseExternal)
//             {
//                 var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
//                 var handler = _registry.ResolveSingle(handlerType)
//                     ?? throw new InvalidOperationException($"No in-memory handler found for {command.GetType().Name} -> {typeof(TResult).Name}");

//                 var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
//                 envelope.MarkPending(handlerName);

//                 TResult result = default!;
//                 var handleCtx = new HandleContext(envelope, handlerName, _services, ct);
//                 await Pipeline.Run(
//                     _subscriberFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(handleCtx, next))),
//                     async () =>
//                     {
//                         try
//                         {
//                             var task = (Task)handlerType.GetMethod("HandleAsync")!
//                                 .Invoke(handler, new object[] { command, envelope, ct })!;
//                             await task.ConfigureAwait(false);
//                             // Task<TResult> result extraction
//                             var t = (dynamic)task;
//                             result = (TResult)t.GetAwaiter().GetResult();
//                             envelope.MarkSuccess(handlerName);
//                         }
//                         catch (Exception ex)
//                         {
//                             envelope.MarkFailed(handlerName, ex.ToString());
//                             throw;
//                         }
//                     });

//                 return result;
//             }
//             else
//             {
//                 var options = new TransportOptions();
//                 await RunTransportPipeline(envelope, options, ct);
//                 var r = await _transport.SendAsync(envelope, options, ct).ConfigureAwait(false);
//                 if (r is { Success: true })
//                     return (TResult?)r.Payload!;

//                 throw new InvalidOperationException(r?.Error ?? "External command<TResult> failed");
//             }
//         }

//         private Task RunPublisherPipeline(MessageEnvelope envelope, CancellationToken ct)
//         {
//             var ctx = new PublishContext(envelope, _services, ct);
//             return Pipeline.Run(
//                 _publisherFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(ctx, next))),
//                 () => Task.CompletedTask
//             );
//         }

//         private Task RunTransportPipeline(MessageEnvelope envelope, TransportOptions options, CancellationToken ct)
//         {
//             var ctx = new TransportContext(envelope, options, ct);
//             return Pipeline.Run(
//                 _transportFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(ctx, next))),
//                 () => Task.CompletedTask
//             );
//         }
//     }

//     public sealed class QueryBus : IQueryBus
//     {
//         private readonly IHandlerRegistry _registry;
//         private readonly IRoutingStrategy _routing;
//         private readonly IEnumerable<IPublisherFilter> _publisherFilters;
//         private readonly IEnumerable<ISubscriberFilter> _subscriberFilters;
//         private readonly IExternalMessageTransport _transport;
//         private readonly IEnumerable<ITransportFilter> _transportFilters;
//         private readonly IServiceProvider _services;

//         public QueryBus(
//             IHandlerRegistry registry,
//             IRoutingStrategy routing,
//             IEnumerable<IPublisherFilter> publisherFilters,
//             IEnumerable<ISubscriberFilter> subscriberFilters,
//             IExternalMessageTransport transport,
//             IEnumerable<ITransportFilter> transportFilters,
//             IServiceProvider services)
//         {
//             _registry = registry;
//             _routing = routing;
//             _publisherFilters = publisherFilters;
//             _subscriberFilters = subscriberFilters;
//             _transport = transport;
//             _transportFilters = transportFilters;
//             _services = services;
//         }

//         public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
//         {
//             var envelope = new MessageEnvelope(query);
//             var ctx = new PublishContext(envelope, _services, ct);
//             await Pipeline.Run(
//                 _publisherFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(ctx, next))),
//                 () => Task.CompletedTask
//             );

//             var decision = _routing.Decide(query);

//             if (!decision.UseExternal)
//             {
//                 var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
//                 var handler = _registry.ResolveSingle(handlerType)
//                     ?? throw new InvalidOperationException($"No in-memory query handler for {query.GetType().Name}");

//                 var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
//                 envelope.MarkPending(handlerName);

//                 TResult result = default!;
//                 var handleCtx = new HandleContext(envelope, handlerName, _services, ct);
//                 await Pipeline.Run(
//                     _subscriberFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(handleCtx, next))),
//                     async () =>
//                     {
//                         try
//                         {
//                             var task = (Task)handlerType.GetMethod("HandleAsync")!
//                                 .Invoke(handler, new object[] { query, envelope, ct })!;
//                             await task.ConfigureAwait(false);
//                             var t = (dynamic)task;
//                             result = (TResult)t.GetAwaiter().GetResult();
//                             envelope.MarkSuccess(handlerName);
//                         }
//                         catch (Exception ex)
//                         {
//                             envelope.MarkFailed(handlerName, ex.ToString());
//                             throw;
//                         }
//                     });

//                 return result;
//             }
//             else
//             {
//                 var options = new TransportOptions();
//                 var tctx = new TransportContext(envelope, options, ct);
//                 await Pipeline.Run(
//                     _transportFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(tctx, next))),
//                     () => Task.CompletedTask
//                 );

//                 var r = await _transport.SendAsync(envelope, options, ct).ConfigureAwait(false);
//                 if (r is { Success: true }) return (TResult?)r.Payload!;
//                 throw new InvalidOperationException(r?.Error ?? "External query failed");
//             }
//         }
//     }

//     public sealed class EventBus : IEventBus
//     {
//         private readonly IHandlerRegistry _registry;
//         private readonly IRoutingStrategy _routing;
//         private readonly IEnumerable<IPublisherFilter> _publisherFilters;
//         private readonly IEnumerable<ISubscriberFilter> _subscriberFilters;
//         private readonly IExternalMessageTransport _transport;
//         private readonly IEnumerable<ITransportFilter> _transportFilters;
//         private readonly IServiceProvider _services;

//         public EventBus(
//             IHandlerRegistry registry,
//             IRoutingStrategy routing,
//             IEnumerable<IPublisherFilter> publisherFilters,
//             IEnumerable<ISubscriberFilter> subscriberFilters,
//             IExternalMessageTransport transport,
//             IEnumerable<ITransportFilter> transportFilters,
//             IServiceProvider services)
//         {
//             _registry = registry;
//             _routing = routing;
//             _publisherFilters = publisherFilters;
//             _subscriberFilters = subscriberFilters;
//             _transport = transport;
//             _transportFilters = transportFilters;
//             _services = services;
//         }

//         public async Task PublishAsync(IEvent @event, CancellationToken ct = default)
//         {
//             var envelope = new MessageEnvelope(@event);
//             var pctx = new PublishContext(envelope, _services, ct);
//             await Pipeline.Run(
//                 _publisherFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(pctx, next))),
//                 () => Task.CompletedTask
//             );

//             var decision = _routing.Decide(@event);

//             if (!decision.UseExternal)
//             {
//                 var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
//                 var handlers = _registry.ResolveMultiple(handlerType).ToArray();
//                 foreach (var handler in handlers)
//                 {
//                     var handlerName = handler.GetType().FullName ?? handler.GetType().Name;
//                     envelope.MarkPending(handlerName);

//                     var hctx = new HandleContext(envelope, handlerName, _services, ct);
//                     await Pipeline.Run(
//                         _subscriberFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(hctx, next))),
//                         async () =>
//                         {
//                             try
//                             {
//                                 var task = (Task)handlerType.GetMethod("HandleAsync")!
//                                     .Invoke(handler, new object[] { @event, envelope, ct })!;
//                                 await task.ConfigureAwait(false);
//                                 envelope.MarkSuccess(handlerName);
//                             }
//                             catch (Exception ex)
//                             {
//                                 envelope.MarkFailed(handlerName, ex.ToString());
//                                 // continue to next handler
//                             }
//                         });
//                 }
//             }
//             else
//             {
//                 var options = new TransportOptions();
//                 var tctx = new TransportContext(envelope, options, ct);
//                 await Pipeline.Run(
//                     _transportFilters.Select(f => (Func<Func<Task>, Func<Task>>)(next => () => f.InvokeAsync(tctx, next))),
//                     () => Task.CompletedTask
//                 );

//                 var r = await _transport.SendAsync(envelope, options, ct).ConfigureAwait(false);
//                 if (r is { Success: false })
//                     throw new InvalidOperationException(r.Error ?? "External publish failed");
//             }
//         }
//     }

//     // ---------------------------
//     // 11) Example decorators (publisher/subscriber) + transport filter
//     // ---------------------------

//     /// <summary>
//     /// Example publisher-side decorator that "would" persist envelopes & handler info to MongoDB.
//     /// Here we only show metadata enrichment and where Mongo calls would go.
//     /// </summary>
//     public sealed class MongoAuditPublisherFilter : IPublisherFilter
//     {
//         public Task InvokeAsync(PublishContext context, Func<Task> next)
//         {
//             // Add metadata often needed for auditing
//             context.Envelope.Metadata["traceId"] = context.Envelope.Metadata.TryGetValue("traceId", out var v)
//                 ? v : Guid.NewGuid().ToString("n");
//             context.Envelope.Metadata["publishedBy"] = context.Envelope.Publisher ?? "unknown";

//             // TODO: Insert context.Envelope into MongoDB "messages" collection here.

//             return next();
//         }
//     }

//     /// <summary>
//     /// Example subscriber-side decorator: updates handler status in Mongo after handling.
//     /// </summary>
//     public sealed class MongoAuditSubscriberFilter : ISubscriberFilter
//     {
//         public async Task InvokeAsync(HandleContext context, Func<Task> next)
//         {
//             // Before handling: ensure handler entry exists (already set to Pending by the bus)
//             // TODO: Upsert handler record with Pending in Mongo

//             try
//             {
//                 await next();
//                 // After success
//                 // TODO: Update handler record to Success in Mongo
//             }
//             catch
//             {
//                 // After failure
//                 // TODO: Update handler record to Failed in Mongo
//                 throw;
//             }
//         }
//     }

//     /// <summary>
//     /// Example transport filter that can compress/trim payload if too large for a provider.
//     /// Replace body with your actual size checks and mutation policy.
//     /// </summary>
//     public sealed class PayloadSizerTransportFilter : ITransportFilter
//     {
//         private readonly int _payloadLimitBytes;

//         public PayloadSizerTransportFilter(int payloadLimitBytes = 200_000) // ~200 KB example
//         {
//             _payloadLimitBytes = payloadLimitBytes;
//         }

//         public Task InvokeAsync(TransportContext context, Func<Task> next)
//         {
//             // Pseudo: check the serialized size (you'll serialize in the real transport).
//             // Here we just react to a marker in metadata for demonstration.
//             if (context.Envelope.Metadata.TryGetValue("approxSize", out var approx) &&
//                 int.TryParse(approx, out var size) && size > _payloadLimitBytes)
//             {
//                 // Example mutation: put payload reference instead of full object
//                 context.Envelope.Metadata["payloadOffloaded"] = "true";
//                 // You might:
//                 // 1) Upload a large blob to storage
//                 // 2) Replace OriginalMessage with a lightweight reference model
//                 // For demo, we only add metadata flag.
//             }

//             return next();
//         }
//     }

//     // ---------------------------
//     // 12) Simple in-memory example types (optional)
//     // ---------------------------

//     // Example messages:
//     public sealed record CreateUser(Guid Id, DateTime MessageOn, string Email) : MessageBase(Id, MessageOn), ICommand;
//     public sealed record CreateUserAndReturnId(string Email) : MessageBase(), ICommand<Guid>;
//     public sealed record GetUserEmail(Guid UserId) : MessageBase(), IQuery<string?>;
//     public sealed record UserCreated(Guid UserId, string Email) : MessageBase(), IEvent;

//     // Example handlers:
//     public sealed class CreateUserHandler : ICommandHandler<CreateUser>
//     {
//         public Task HandleAsync(CreateUser command, MessageEnvelope envelope, CancellationToken ct)
//         {
//             // ... do work
//             return Task.CompletedTask;
//         }
//     }

//     public sealed class CreateUserAndReturnIdHandler : ICommandHandler<CreateUserAndReturnId, Guid>
//     {
//         public Task<Guid> HandleAsync(CreateUserAndReturnId command, MessageEnvelope envelope, CancellationToken ct)
//         {
//             var id = Guid.NewGuid();
//             return Task.FromResult(id);
//         }
//     }

//     public sealed class GetUserEmailHandler : IQueryHandler<GetUserEmail, string?>
//     {
//         public Task<string?> HandleAsync(GetUserEmail query, MessageEnvelope envelope, CancellationToken ct)
//             => Task.FromResult<string?>("someone@example.com");
//     }

//     public sealed class UserCreatedHandlerA : IEventHandler<UserCreated>
//     {
//         public Task HandleAsync(UserCreated e, MessageEnvelope envelope, CancellationToken ct)
//             => Task.CompletedTask;
//     }

//     public sealed class UserCreatedHandlerB : IEventHandler<UserCreated>
//     {
//         public Task HandleAsync(UserCreated e, MessageEnvelope envelope, CancellationToken ct)
//             => Task.CompletedTask;
//     }
// }
