namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Keeps a record of all events that have been raised in the system.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public interface IEventRepository<TMessageId, TEntityKey> : IMessageRepository<TMessageId, IEvent<TMessageId>>
    where TMessageId : struct
    where TEntityKey : struct
{
    /// <summary>
    /// Get all events for an entity key.
    /// </summary>
    /// <param name="key">(eg UserId, OrderId, BlogId)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// All events for the key, sorted by MessageOnUtc ascending order.
    /// If no events are found for the key, it will return an empty List
    /// </returns>
    Task<List<IEvent<TMessageId, TEntityKey>>> GetEntityMessagesAsync(TEntityKey key, CancellationToken cancellationToken = default);
}
