using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus.Transport;

public interface IMessageRouter
{
    /// <summary>
    /// Execute Command messages and wait for it to finish processing.
    /// </summary>
    Task ExecuteAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage;

    /// <summary>
    /// Execute Command or Query messages that expect a response.
    /// </summary>
    Task<TResponse> ExecuteAsync<TMessage, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage<TResponse>;

    /// <summary>
    /// Send a Command or Event message, fire-and-forget from caller perspective.
    /// </summary>
    Task SendAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage;

    /// <summary>
    /// Send multiple Command or Event messages, fire-and-forget from caller perspective.
    /// </summary>
    Task SendManyAsync<TMessage>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TMessage : IMessage;
}
