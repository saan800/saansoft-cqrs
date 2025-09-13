using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus.External;

public interface IExternalMessageProcessor
{
    /// <summary>
    /// Publish 0-n messages to an external message transport, without waiting for a response. (i.e. fire-and-forget)
    /// Used with <see cref="IEvent"/> and <see cref="ICommand"/> via IMessageBus.SendAsync.
    /// </summary>
    /// <remarks>
    /// Success/failure/exceptions are an indication that the message was published to the external transport,
    /// not if the message(s) were actually processed successfully.
    /// </remarks>
    Task PublishExternallyAsync<TMessage>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TMessage : IMessage;

    /// <summary>
    /// Publish a message to an external message transport, and wait for a response (but not a TResult).
    /// Used with <see cref="ICommand"/> via IMessageBus.ExecuteAsync.
    /// </summary>
    /// <remarks>
    /// Success/failure/exceptions are an indications of the handler's execution status.
    /// </remarks>
    Task PublishExternallyAndWaitAsync<TMessage>(MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage;


    /// <summary>
    /// Publish a message to an external message transport, and wait for a response and the TResult.
    /// Used with <see cref="ICommand{TResult}"/> and <see cref="IQuery{TResult}"/>.
    /// </summary>
    /// <remarks>
    /// Success/failure/exceptions are an indications of the handler's execution status.
    /// </remarks>
    Task<TResult> PublishExternallyAndWaitForResultsAsync<TMessage, TResult>(
        MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage;
}
