using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Interface for an external pub/sub provider (eg Azure Service Bus, AWS SNS/SQS, RabbitMq)
/// </summary>
public interface IExternalMessageTransport
{
    /// <summary>
    /// Publish a message to the external transport
    ///
    /// The message will be serialized by the transport implementation.
    /// </summary>
    /// <remarks>
    /// If WaitForExecution=false, the IExternalMessageTransport should still return a ExternalResult,
    /// but Success indicates that the message was published successfully, rather than handled successfully.
    /// </remarks>
    /// <returns>
    /// A null ExternalResult indicates an issue happened while publishing the message, and will result
    /// in exceptions.
    /// </returns>
    Task<ExternalResult?> PublishAsync(MessageEnvelope envelope, IExternalTransportOptions options, CancellationToken ct);

    /// <summary>
    /// Publish messages of the same type to the external transport.
    ///
    /// Only used on messages where WaitForExecution=false.
    ///
    /// The messages will be serialized by the transport implementation.
    ///
    /// Any batching of messages will be handled by the transport implementation.
    /// </summary>
    /// <remarks>
    /// If WaitForExecution=false, the IExternalMessageTransport should still return a ExternalResult,
    /// but Success indicates that the message was published successfully, rather than handled successfully.
    /// </remarks>
    /// <returns>
    /// A null ExternalResult indicates an issue happened while publishing the messages, and will result
    /// in exceptions.
    /// </returns>
    Task<ExternalResult?> PublishManyAsync(IReadOnlyCollection<MessageEnvelope> envelopes, IExternalTransportOptions options, CancellationToken ct);
}
