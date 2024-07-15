namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Base interface with common methods for all message stores
/// You should never directly inherit from IMessageRepository
/// Use <see cref="ICommandRepository{TMessageId}"/>, <see cref="IEventRepository{TMessageId,TEntityKey}"/> or <see cref="IQueryRepository{TMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageRepository<TMessageId, in TMessage>
    where TMessageId : struct
    where TMessage : IBaseMessage<TMessageId>
{
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
}
