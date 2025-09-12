namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Enrich messages have been subscribed to from an external message transport.
/// </summary>
/// <remarks>
/// It is likely that any IExternalTransportPublisherMiddleware that alters a message will require a
/// matching IExternalTransportSubscriberMiddleware to get the message back into its original state.
/// (eg Serialize and Deserialize a message)
/// </remarks>
public interface IExternalTransportSubscriberMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(TransportContext context, Func<Task> next, CancellationToken ct);
}
