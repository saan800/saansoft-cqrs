using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Interface for an external pub/sub provider (eg Azure Service Bus, AWS SNS/SQS, RabbitMq)
/// </summary>
public interface IExternalMessageProvider
{
    /// <summary>
    /// Publish a message to the external transport and return the response with payload
    ///
    /// The message will be serialized by the transport implementation.
    ///
    /// There should only be a single handler for messages that expect a response.
    /// </summary>
    /// <remarks>
    /// Should always wait for execution
    /// </remarks>
    /// <returns>
    /// The ExternalResponse&lt;TResponse&gt; with either success=true and payload populated,
    /// or error/exception from handling the message
    /// 
    /// A null ExternalResponse indicates an issue happened while publishing the message, and will result
    /// in an exception.
    /// </returns>
    Task<ExternalResponse<TResponse>?> PublishAndWaitForResponseAsync<TResponse>(
        MessageEnvelope envelope,
        IExternalMessageProviderOptions options,
        CancellationToken ct);

    /// <summary>
    /// Publish message(s) of the same type to the external transport.
    ///
    /// The messages will be serialized by the transport implementation.
    ///
    /// Any batching of messages will be handled by the transport implementation.
    /// </summary>
    /// <remarks>
    /// If WaitForExecution=false, the IExternalMessageProvider should still return a ExternalResponse,
    /// but Success indicates that the message was published successfully, rather than handled successfully.
    /// </remarks>
    /// <returns>
    /// The ExternalResponse with either success=true or error/exception from handling the message
    /// 
    /// A null ExternalResponse indicates an issue happened while publishing the messages, and will result
    /// in exceptions.
    /// </returns>
    Task<ExternalResponse?> PublishManyAsync(
        IReadOnlyCollection<MessageEnvelope> envelopes,
        IExternalMessageProviderOptions options,
        CancellationToken ct);
}
