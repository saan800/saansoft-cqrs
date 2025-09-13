namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Enrich messages that will be published externally.
/// (eg. serialise the message ready to send over the wire)
/// </summary>
/// <remarks>
/// It is likely that any IExternalTransportPublisherMiddleware that alters a message will require a
/// matching IExternalTransportSubscriberMiddleware to get the message back into its original state.
/// (eg Serialize and Deserialize a message)
/// </remarks>
/// <typeparam name="TMessage">
/// Create the middleware against the type of message that you want. eg.
/// To run against all messages of all types use ISubscriberMiddleware&lt;IMessage&gt;.
/// To run against all events use ISubscriberMiddleware&lt;IEvent&gt;.
/// To run against a specific custom message type use ISubscriberMiddleware&lt;UserUpdatedEvent&gt;.
/// </typeparam>
public interface IExternalTransportPublisherMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(TransportContext context, Func<Task> next, CancellationToken ct);
}
