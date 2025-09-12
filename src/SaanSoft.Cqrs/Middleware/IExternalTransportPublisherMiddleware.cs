namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Enrich messages that will be published externally.
/// Add the response message queue name to the IExternalTransportOptions.Headers,
/// or serialise the message ready to send over the wire.
/// </summary>
/// <remarks>
/// It is likely that any IExternalTransportPublisherMiddleware that alters a message will require a
/// matching IExternalTransportSubscriberMiddleware to get the message back into its original state.
/// (eg Serialize and Deserialize a message)
/// </remarks>
public interface IExternalTransportPublisherMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(TransportContext context, Func<Task> next, CancellationToken ct);
}
