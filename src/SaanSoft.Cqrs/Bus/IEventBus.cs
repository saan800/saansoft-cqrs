using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IEventBus<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Put the event onto the queue
    /// It will not return any indication if the event was successfully executed or not
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>;

    /// <summary>
    /// Put the events onto the queue
    /// It will not return any indication if the events were successfully executed or not
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>;
}
