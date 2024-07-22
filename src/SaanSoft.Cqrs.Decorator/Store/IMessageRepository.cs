namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Base interface with common methods for all message stores
/// You should never directly inherit from IMessageRepository
/// Use <see cref="ICommandRepository"/>, <see cref="IEventRepository{TEntityKey}"/> or <see cref="IQueryRepository"/> instead
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageRepository<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Get a message by its Id
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a message to the Store
    /// </summary>
    /// <remarks>
    /// It will not store messages in replay mode
    /// </remarks>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InsertAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Store the class of the handler of a message, and whether the message succeeded or failed.
    /// Used with <see ref="Store[Command|Event|Query]HandlerDecorator"/>.
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
