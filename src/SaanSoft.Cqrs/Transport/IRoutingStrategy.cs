namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Strategy for deciding on how to route messages to different destinations based on their content or type.
/// </summary>
public interface IRoutingStrategy
{
    /// <summary>
    /// Decide if need to route the message externally or use in memory (default) transport.
    /// </summary>
    /// <exception cref="ApplicationException">Any critical issues which arise during routing decision making.</exception>
    bool IsExternalMessage<TMessage>(TMessage message) where TMessage : IMessage;
}
