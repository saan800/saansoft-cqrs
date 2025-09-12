namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Enrich messages that have been published
/// </summary>
/// <typeparam name="TMessage">
/// Create the middleware against the type of message that you want. eg.
/// To run against all messages of all types use ISubscriberMiddleware&lt;IMessage&gt;.
/// To run against all events use ISubscriberMiddleware&lt;IEvent&gt;.
/// To run against a specific custom message type use ISubscriberMiddleware&lt;UserUpdatedEvent&gt;.
/// </typeparam>
public interface IPublisherMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(PublishContext context, Func<Task> next, CancellationToken ct);
}
