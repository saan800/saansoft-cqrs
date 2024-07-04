namespace SaanSoft.Cqrs.Decorator.Store;

public interface IMessageHandlerRepository<in TMessageId>
    where TMessageId : struct
{
    /// <summary>
    /// Store the class of the handler of a message, and whether the message succeeded or failed.
    /// Used with <see ref="Store[Command|Event|Query]HandlerDecorator"/>
    /// </summary>
    /// <param name="id">Message Id being handled</param>
    /// <param name="handlerType">Class that handles the message</param>>
    /// <param name="exception">
    /// Exception thrown when the message was handled by the handler..
    /// Providing this will indicate that the message failed in the handler.
    /// If exception is null, then indicates the  message succeeded in the handler.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertHandlerAsync(TMessageId id, Type handlerType, Exception? exception = null, CancellationToken cancellationToken = default);
}
