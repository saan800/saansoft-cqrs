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
}
