// // namespace SaanSoft.Cqrs;

// // ------------------------------------------------------------
// // 1) Abstractions: Messages & Handlers
// // ------------------------------------------------------------

// // public interface IMessage
// // {
// // Guid Id { get; }
// // DateTime EventOn { get; } // UTC
// // string CorrelationId { get; }
// // string? AuthenticationId { get; }
// // }


// // public interface ICommand : IMessage { }


// // public interface IEvent : IMessage { }


// // public interface IQuery<TResult> : IMessage { }


// // public interface ICommandHandler<TCommand> where TCommand : ICommand
// // {
// // Task HandleAsync(TCommand command, CancellationToken ct = default);
// // }


// // public interface IEventHandler<TEvent> where TEvent : IEvent
// // {
// // Task HandleAsync(TEvent @event, CancellationToken ct = default);
// // }


// // public interface IQueryHandler<TQuery, TResult>
// // where TQuery : IQuery<TResult>
// // {
// // Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
// // }


// // // A simple base record for convenience
// // public abstract record MessageBase(
// // Guid Id,
// // DateTime EventOn,
// // string CorrelationId,
// // string? AuthenticationId
// // ) : IMessage;



// // ------------------------------------------------------------
// // 2) Envelope + Transport Contracts
// // ------------------------------------------------------------



// // using System.Text.Json;


// // public sealed record MessageEnvelope(
// // string Type, // CLR type (AssemblyQualifiedName or mapped alias)
// // string Body, // Serialized payload (JSON)
// // string CorrelationId,
// // string? AuthenticationId,
// // string Publisher, // e.g. "app://orders-service"
// // string? Subscriber, // set by subscriber when processed
// // DateTime PublishedAtUtc,
// // IDictionary<string, string>? Headers = null
// // )
// // {
// // public static MessageEnvelope Wrap<T>(T message, string publisher, IDictionary<string,string>? headers = null)
// // where T : IMessage
// // => new(
// // Type: typeof(T).AssemblyQualifiedName!,
// // Body: JsonSerializer.Serialize(message),
// // CorrelationId: message.CorrelationId,
// // AuthenticationId: message.AuthenticationId,
// // Publisher: publisher,
// // Subscriber: null,
// // PublishedAtUtc: DateTime.UtcNow,
// // Headers: headers
// // );
// // }


// // public interface ISerializer
// // {
// // string Serialize<T>(T value);
// // T Deserialize<T>(string json);
// // object Deserialize(string json, Type type);
// // }


// // public sealed class SystemTextJsonSerializer : ISerializer
// // {
// // private readonly JsonSerializerOptions _options;
// // public SystemTextJsonSerializer(JsonSerializerOptions? options = null)
// // => _options = options ?? new JsonSerializerOptions
// // {
// // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
// // WriteIndented = false
// // };


// // public string Serialize<T>(T value) => System.Text.Json.JsonSerializer.Serialize(value, _options);
// // public T Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json, _options)!;
// // public object Deserialize(string json, Type type) => System.Text.Json.JsonSerializer.Deserialize(json, type, _options)!;
// // }

// // ------------------------------------------------------------
// // 3) Middleware (Decorators) for Publishing/Handling
// // ------------------------------------------------------------


// // public delegate Task MessageHandlerDelegate<TMessage>(TMessage message, CancellationToken ct) where TMessage : IMessage;


// // public interface IMessageMiddleware<TMessage> where TMessage : IMessage
// // {
// //     Task InvokeAsync(TMessage message, MessageHandlerDelegate<TMessage> next, CancellationToken ct);
// // }


// // // Example middlewares
// // public sealed class LoggingMiddleware<TMessage> : IMessageMiddleware<TMessage> where TMessage : IMessage
// // {
// // private readonly ILogger<LoggingMiddleware<TMessage>> _logger;
// // public LoggingMiddleware(ILogger<LoggingMiddleware<TMessage>> logger) => _logger = logger;
// // public async Task InvokeAsync(TMessage message, MessageHandlerDelegate<TMessage> next, CancellationToken ct)
// // {
// // _logger.LogInformation("Handling {Type} {Id} corr {Corr}", typeof(TMessage).Name, message.Id, message.CorrelationId);
// // try
// // {
// // await next(message, ct);
// // _logger.LogInformation("Handled {Type} {Id}", typeof(TMessage).Name, message.Id);
// // }
// // catch (Exception ex)
// // {
// // _logger.LogError(ex, "Error handling {Type} {Id}", typeof(TMessage).Name, message.Id);
// // throw;
// // }
// // }
// // }


// // public sealed class AuthenticationMiddleware<TMessage> : IMessageMiddleware<TMessage> where TMessage : IMessage
// // {
// // private readonly IAuthenticationContext _auth;
// // public AuthenticationMiddleware(IAuthenticationContext auth) => _auth = auth;
// // public Task InvokeAsync(TMessage message, MessageHandlerDelegate<TMessage> next, CancellationToken ct)
// // {
// // _auth.Set(message.AuthenticationId);
// // return next(message, ct);
// // }
// //}




// // ------------------------------------------------------------
// // 5) Local In-Memory Bus (for same-process handling)
// // ------------------------------------------------------------


// // namespace Cqrs.Bus;


// // using Cqrs.Core;
// // using Cqrs.Middleware;
// // using Microsoft.Extensions.DependencyInjection;


// // public interface IMessageBus
// // {
// // Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand;
// // Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
// // Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResult>;
// // }

// // public sealed class InMemoryBus : IMessageBus
// // {
// // private readonly IServiceProvider _sp;
// // private readonly IEnumerable<object> _middlewares; // open generics are tough, we filter at runtime


// // public InMemoryBus(IServiceProvider sp, IEnumerable<object> middlewares)
// // {
// // _sp = sp;
// // _middlewares = middlewares;
// // }

// // public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
// // => InvokePipeline(command, async (msg, token) =>
// // {
// // var handler = _sp.GetRequiredService<ICommandHandler<TCommand>>();
// // await handler.HandleAsync(msg, token);
// // }, ct);


// // public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent
// // => InvokePipeline(@event, async (msg, token) =>
// // {
// // var handlers = _sp.GetServices<IEventHandler<TEvent>>();
// // foreach (var h in handlers)
// // await h.HandleAsync(msg, token);
// // }, ct);


// // public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResult>
// // => InvokePipeline(query, async (msg, token) =>
// // {
// // var handler = _sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
// // return await handler.HandleAsync(msg, token);
// // }, ct);


// // private async Task InvokePipeline<TMessage>(TMessage message, Func<TMessage, CancellationToken, Task> terminal, CancellationToken ct)
// // where TMessage : IMessage
// // {
// // var chain = BuildPipeline(message, terminal, ct);
// // await chain(message, ct);
// // }


// // private async Task<TResult> InvokePipeline<TMessage, TResult>(TMessage message, Func<TMessage, CancellationToken, Task<TResult>> terminal, CancellationToken ct)
// // where TMessage : IMessage
// // {
// // TResult? response = default!;
// // var chain = BuildPipeline(message, async (m, token) => { response = await terminal(m, token); }, ct);
// // await chain(message, ct);
// // return response!;
// // }


// // private MessageHandlerDelegate<TMessage> BuildPipeline<TMessage>(TMessage message, Func<TMessage, CancellationToken, Task> terminal, CancellationToken ct)
// // where TMessage : IMessage
// // {
// // MessageHandlerDelegate<TMessage> next = (m, token) => terminal(m, token);


// // foreach (var mw in _middlewares.Reverse())
// // {
// // if (mw is IMessageMiddleware<TMessage> typed)
// // {
// // var currentNext = next;
// // next = (m, token) => typed.InvokeAsync(m, currentNext, token);
// // }
// // }


// // return next;
// // }
// // }




// // ------------------------------------------------------------
// // 7) Message Logging (MongoDB) + DynamoDB stub
// // ------------------------------------------------------------


// // namespace Cqrs.Logging;


// // using Cqrs.Transport;
// // using MongoDB.Bson;
// // using MongoDB.Bson.Serialization.Attributes;
// // using MongoDB.Driver;


// // public sealed class MessageLog
// // {
// // [BsonId]
// // public ObjectId Id { get; set; }

// // public string MessageId { get; set; } = default!; // original IMessage.Id
// // public string CorrelationId { get; set; } = default!;
// // public string? AuthenticationId { get; set; }


// // public string Type { get; set; } = default!;
// // public string Publisher { get; set; } = default!;
// // public string? Subscriber { get; set; }


// // public DateTime PublishedAtUtc { get; set; }
// // public DateTime? ProcessedAtUtc { get; set; }
// // public string Body { get; set; } = default!; // serialized
// // public IDictionary<string, string>? Headers { get; set; }


// // public string Status { get; set; } = "Published"; // Published, Processed, Failed
// // public string? Error { get; set; }
// // }


// // public interface IMessageLogStore
// // {
// // Task LogPublishedAsync(MessageEnvelope envelope, string messageId, CancellationToken ct = default);
// // Task LogProcessedAsync(string messageId, string? subscriber, string status, string? error, CancellationToken ct = default);
// // }

// // public MongoMessageLogStore(IMongoDatabase db, string collectionName = "message_logs")
// // {
// // _col = db.GetCollection<MessageLog>(collectionName);
// // CreateIndexes(_col);
// // }


// // private static void CreateIndexes(IMongoCollection<MessageLog> col)
// // {
// // var keys = Builders<MessageLog>.IndexKeys
// // .Ascending(x => x.MessageId)
// // .Ascending(x => x.CorrelationId)
// // .Ascending(x => x.Publisher)
// // .Ascending(x => x.PublishedAtUtc);


// // col.Indexes.CreateMany(new[]
// // {
// // new CreateIndexModel<MessageLog>(keys, new CreateIndexOptions { Name = "message_core" }),
// // new CreateIndexModel<MessageLog>(Builders<MessageLog>.IndexKeys.Ascending(x => x.Status), new CreateIndexOptions { Name = "status" }),
// // });
// // }


// // public Task LogPublishedAsync(MessageEnvelope envelope, string messageId, CancellationToken ct = default)
// // {
// // var doc = new MessageLog
// // {
// // MessageId = messageId,
// // CorrelationId = envelope.CorrelationId,
// // AuthenticationId = envelope.AuthenticationId,
// // Type = envelope.Type,
// // Publisher = envelope.Publisher,
// // Subscriber = envelope.Subscriber,
// // PublishedAtUtc = envelope.PublishedAtUtc,
// // Body = envelope.Body,
// // Headers = envelope.Headers,
// // Status = "Published"
// // };
// // return _col.InsertOneAsync(doc, cancellationToken: ct);
// // }


// // public Task LogProcessedAsync(string messageId, string? subscriber, string status, string? error, CancellationToken ct = default)
// // {
// // var update = Builders<MessageLog>.Update
// // .Set(x => x.Subscriber, subscriber)
// // .Set(x => x.ProcessedAtUtc, DateTime.UtcNow)
// // .Set(x => x.Status, status)
// // .Set(x => x.Error, error);


// // return _col.UpdateOneAsync(x => x.MessageId == messageId, update, cancellationToken: ct);
// // }
// // }


// // // DynamoDB stub for parity (implement as needed)
// // public sealed class DynamoDbMessageLogStore : IMessageLogStore
// // {
// // public Task LogPublishedAsync(MessageEnvelope envelope, string messageId, CancellationToken ct = default) => Task.CompletedTask;
// // public Task LogProcessedAsync(string messageId, string? subscriber, string status, string? error, CancellationToken ct = default) => Task.CompletedTask;
// // }




// // ------------------------------------------------------------
// // 8) Dispatcher for Subscribers (bridges external transport to handlers)
// // ------------------------------------------------------------


// namespace Cqrs.Dispatching;


// using Cqrs.Context;
// using Cqrs.Core;
// using Cqrs.Logging;
// using Cqrs.Transport;
// using Microsoft.Extensions.DependencyInjection;

// public sealed class EnvelopeDispatcher
// {
//     private readonly IServiceProvider _sp;
//     private readonly ISerializer _serializer;
//     private readonly IMessageLogStore _logStore;


//     public EnvelopeDispatcher(IServiceProvider sp, ISerializer serializer, IMessageLogStore logStore)
//     {
//         _sp = sp; _serializer = serializer; _logStore = logStore;
//     }


//     public async Task DispatchAsync(MessageEnvelope env, CancellationToken ct = default)
//     {
//         var type = Type.GetType(env.Type, throwOnError: true)!;
//         var payload = _serializer.Deserialize(env.Body, type);


//         if (payload is not IMessage message)
//             throw new InvalidOperationException($"Envelope payload is not IMessage: {env.Type}");


//         // set contexts
//         var corr = _sp.GetRequiredService<ICorrelationIdProvider>();
//         corr.Set(message.CorrelationId);
//         var auth = _sp.GetRequiredService<IAuthenticationContext>();
//         auth.Set(message.AuthenticationId);


//         // dispatch based on runtime type
//         try
//         {
//             var bus = _sp.GetRequiredService<Cqrs.Bus.IMessageBus>();


//             switch (payload)
//             {
//                 case ICommand cmd:
//                     // Reflective generic invocation
//                     var sendMethod = typeof(Cqrs.Bus.IMessageBus).GetMethod(nameof(Cqrs.Bus.IMessageBus.ExecuteAsync))!;
//                     await (Task)sendMethod.MakeGenericMethod(type).Invoke(bus, new object?[] { payload, ct })!;
//                     break;
//                 case IEvent evt:
//                     var pubMethod = typeof(Cqrs.Bus.IMessageBus).GetMethod(nameof(Cqrs.Bus.IMessageBus.PublishAsync))!;
//                     await (Task)pubMethod.MakeGenericMethod(type).Invoke(bus, new object?[] { payload, ct })!;
//                     break;
//                 case var q when ImplementsGeneric(type, typeof(IQuery<>)):
//                     // Queries over external bus are rare; ignore response here
//                     var qType = type;
//                     var tResp = qType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>)).GetGenericArguments()[0];
//                     var qMethod = typeof(Cqrs.Bus.IMessageBus).GetMethod(nameof(Cqrs.Bus.IMessageBus.QueryAsync))!;
//                     // Invoke but discard result
//                     var task = (Task)qMethod.MakeGenericMethod(qType, tResp).Invoke(bus, new object?[] { payload, ct })!;
//                     await task;
//                     break;
//                 default:
//                     throw new NotSupportedException($"Unsupported message type: {type}");
//             }


//             await _logStore.LogProcessedAsync(((IMessage)payload).Id.ToString(), env.Subscriber, "Processed", null, ct);
//         }
//         catch (Exception ex)
//         {
//             await _logStore.LogProcessedAsync(((IMessage)payload).Id.ToString(), env.Subscriber, "Failed", ex.Message, ct);
//             throw;
//         }
//     }


//     private static bool ImplementsGeneric(Type type, Type openGeneric)
//     => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
// }
