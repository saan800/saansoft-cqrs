namespace SaanSoft.Cqrs.Handlers;


public interface IHandleMessage<TMessage> where TMessage : IMessage
{
    /// <summary>
    /// Process the command/event, does not return a response
    /// </summary>
    Task HandleAsync(TMessage message, CancellationToken ct = default);
}

public interface IHandleMessage<in TMessage, TResponse>
    where TMessage : IMessage<TResponse>
{
    /// <summary>
    /// Process the message
    ///
    /// Returns the response of the command/query
    /// </summary>
    Task<TResponse> HandleAsync(TMessage message, CancellationToken ct = default);
}
