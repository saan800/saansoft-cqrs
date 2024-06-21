using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Base interface with common methods for all message stores
/// You should never directly inherit from IMessageStore
/// Use <see cref="ICommandStore{TMessageId}"/>, <see cref="IEventStore{TMessageId, TEntityKey}"/> or <see cref="IQueryStore{IMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageStore<TMessageId, in TMessage>
    where TMessageId : struct
    where TMessage : IMessage<TMessageId>
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

    /// <summary>
    /// TODO: do we need this still? or is singular insert ok???
    /// Add messages to the Store
    /// </summary>
    /// <remarks>
    /// It will not store messages in replay mode
    /// </remarks>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InsertManyAsync(IEnumerable<TMessage> message, CancellationToken cancellationToken = default);
}