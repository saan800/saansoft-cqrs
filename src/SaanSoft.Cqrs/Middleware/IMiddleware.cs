namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Enrich messages envelopes
/// </summary>
public interface IMiddleware
{
    /// <summary>
    /// Check if this middleware is valid to run for TMessage.
    /// </summary>
    /// <typeparam name="TMessage">
    /// Create the middleware against the type of message that you want. eg.
    ///
    /// To run against all messages of all types just return true
    ///
    /// To run against all events use extension method: "typeof(TMessage).IsEvent()"
    ///
    /// To run against a specific custom message type use: "typeof(TMessage) == typeof(MyMessage)"
    ///
    /// To run against all messages in an assembly use extension method:
    ///   "typeof(TMessage).GetAssemblyName() == typeof(MyMessage).GetAssemblyName()"
    /// </typeparam>
    bool IsValidForMessage<TMessage>() where TMessage : IMessage;

    /// <summary>
    /// Run the middleware
    /// </summary>
    Task InvokeAsync<TMessage>(MessageEnvelope envelope, Func<Task> next, CancellationToken ct)
        where TMessage : IMessage;
}


// TODO: add this to external router config options
// / <summary>
// / Enrich messages that will be published externally.
// / (eg. serialise the message ready to send over the wire)
// / </summary>
// / <remarks>
// / It is likely that any IExternalTransportPublisherMiddleware that alters a message will require a
// / matching IExternalTransportSubscriberMiddleware to get the message back into its original state.
// / (eg Serialize and Deserialize a message)
// / </remarks>
// / <typeparam name="TMessage">
// / Create the middleware against the type of message that you want. eg.
// / To run against all messages of all types use ISubscriberMiddleware&lt;IMessage&gt;.
// / To run against all events use ISubscriberMiddleware&lt;IEvent&gt;.
// / To run against a specific custom message type use ISubscriberMiddleware&lt;UserUpdatedEvent&gt;.
// / </typeparam>
