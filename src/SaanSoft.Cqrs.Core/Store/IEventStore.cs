using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Store;

public interface IEventStore<TId, TEntityKey>
{
    /// <summary>
    /// Get all events for an entity key
    /// </summary>
    /// <param name="key">(eg UserId, OrderId, BlogId)</param>
    /// <returns></returns>
    Task<IEnumerable<IEvent<TId, TEntityKey>>> GetEventsAsync(TEntityKey key);

    /// <summary>
    /// Save a new event for an entity
    /// </summary>
    /// <param name="key"></param>
    /// <param name="evt"></param>
    /// <returns></returns>
    Task SaveEventAsync(TEntityKey key, IEvent<TId, TEntityKey> evt);

    /// <summary>
    /// Save new events for an entity
    /// </summary>
    /// <param name="key">(eg UserId, OrderId, BlogId)</param>
    /// <param name="events"></param>
    /// <returns></returns>
    Task SaveEventsAsync(TEntityKey key, IEnumerable<IEvent<TId, TEntityKey>> events);
}
