using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Middleware;

// public sealed class LoggingMiddleware<TMessage>(ILogger<LoggingMiddleware<TMessage>> logger) :
//     IPublishMiddleware<TMessage>,
//     ISubscriberMiddleware<TMessage>
//     where TMessage : IMessage
// {
//     private readonly ILogger<LoggingMiddleware<TMessage>> logger = logger;

//     public async Task InvokeAsync(TMessage message, PublishHandlerDelegate<TMessage> next, CancellationToken ct)
//     {
//         using (logger.BeginScope(message.BuildLoggingScopeData()))
//         {
//             try
//             {
//                 logger.LogInformation(
//                     "Publishing {Type} {Id} {CorrelationId}",
//                     typeof(TMessage).GetTypeFullName(),
//                     message.Id,
//                     message.CorrelationId
//                 );
//                 await next(message, ct);
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(
//                     ex,
//                     "Error publishing {Type} {Id} {CorrelationId}",
//                     typeof(TMessage).GetTypeFullName(),
//                     message.Id,
//                     message.CorrelationId
//                 );
//                 throw;
//             }
//         }
//     }

//     public async Task InvokeAsync(TMessage message, SubscriberHandlerDelegate<TMessage> next, CancellationToken ct)
//     {
//         using (logger.BeginScope(message.BuildLoggingScopeData()))
//         {
//             try
//             {
//                 logger.LogInformation(
//                     "Handling {Type} {Id} {CorrelationId}",
//                     typeof(TMessage).GetTypeFullName(),
//                     message.Id,
//                     message.CorrelationId
//                 );
//                 await next(message, ct);
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(
//                     ex,
//                     "Error handling {Type} {Id} {CorrelationId}",
//                     typeof(TMessage).GetTypeFullName(),
//                     message.Id,
//                     message.CorrelationId
//                 );
//                 throw;
//             }
//         }
//     }
// }
