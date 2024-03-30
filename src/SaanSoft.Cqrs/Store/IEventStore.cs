using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

public interface IEventStore<TMessageId>
{
    /// <summary>
    /// Get all events for an entity key
    /// </summary>
    /// <param name="key">(eg UserId, OrderId, BlogId)</param>
    /// <returns></returns>
    Task<IEnumerable<IEvent<TMessageId, TEntityKey>>> GetAsync<TEntityKey>(TEntityKey key);

    /// <summary>
    /// Save a new event
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    Task InsertAsync<TEvent>(TEvent evt) where TEvent : IEvent<TMessageId>;

    /// <summary>
    /// Save new events
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    Task InsertAsync<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent<TMessageId>;
}
