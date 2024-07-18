namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Base interface with common methods for all message stores
/// You should never directly inherit from IMessageRepository
/// Use <see cref="ICommandRepository"/>, <see cref="IEventRepository{TEntityKey}"/> or <see cref="IQueryRepository"/> instead
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageRepository<in TMessage>
    where TMessage : IMessage
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
