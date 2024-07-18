namespace SaanSoft.Cqrs.Decorator.Store;

public interface IMessageHandlerRepository
{
    /// <summary>
    /// Store the class of the handler of a message, and whether the message succeeded or failed.
    /// Used with <see ref="Store[Command|Event|Query]HandlerDecorator"/>.
    /// 
    /// TODO should be stored on the message metadata in the ... model and key
    /// </summary>
    /// <param name="id">Message Id being handled</param>
    /// <param name="handlerType">Class that handled the message</param>>
    /// <param name="exception">
    /// Exception thrown when the message was handled by the handler..
    /// Providing this will indicate that the message failed in the handler.
    /// If exception is null, then indicates the  message succeeded in the handler.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertHandlerAsync(Guid id, Type handlerType, Exception? exception = null, CancellationToken cancellationToken = default);
}
