using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Transport;


// public sealed class EnvelopeDispatcher(IServiceProvider sp, ITransportSerializer serializer)
// {
//     private readonly IServiceProvider _sp = sp;
//     private readonly ITransportSerializer _serializer = serializer;
//     //private readonly IMessageLogStore _logStore = logStore;

//     public async Task DispatchAsync(MessageEnvelope env, CancellationToken ct = default)
//     {
//         var type = Type.GetType(env.Type, throwOnError: true)!;
//         var payload = _serializer.Deserialize(env.Body, type);


//         if (payload is not IMessage message)
//             throw new InvalidOperationException($"Envelope payload is not IMessage: {env.Type}");


//         // set contexts
//         // var corr = _sp.GetRequiredService<ICorrelationIdProvider>();
//         // corr.Set(message.CorrelationId);
//         // var auth = _sp.GetRequiredService<IAuthenticationContext>();
//         // auth.Set(message.AuthenticationId);


//         // dispatch based on runtime type
//         try
//         {
//             var bus = _sp.GetRequiredService<Bus.IMessageBus>();

//             switch (payload)
//             {
//                 case ICommand cmd:
//                     // Reflective generic invocation
//                     var sendMethod = typeof(Bus.IMessageBus).GetMethod(nameof(Bus.IMessageBus.ExecuteAsync))!;
//                     await (Task)sendMethod.MakeGenericMethod(type).Invoke(bus, [payload, ct])!;
//                     break;
//                 case IEvent evt:
//                     var pubMethod = typeof(Bus.IMessageBus).GetMethod(nameof(Bus.IMessageBus.PublishAsync))!;
//                     await (Task)pubMethod.MakeGenericMethod(type).Invoke(bus, [payload, ct])!;
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

//             //await _logStore.LogProcessedAsync(((IMessage)payload).Id.ToString(), env.Subscriber, "Processed", null, ct);
//         }
//         catch (Exception)
//         {
//             //await _logStore.LogProcessedAsync(((IMessage)payload).Id.ToString(), env.Subscriber, "Failed", ex.Message, ct);
//             throw;
//         }
//     }


//     private static bool ImplementsGeneric(Type type, Type openGeneric)
//         => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
// }
