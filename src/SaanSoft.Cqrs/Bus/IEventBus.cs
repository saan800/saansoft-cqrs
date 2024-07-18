namespace SaanSoft.Cqrs.Bus;

public interface IEventBus
{
    /// <summary>
    /// Put the event onto the queue.
    /// It will not return any indication if the event was successfully executed or not.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    /// <summary>
    /// Put the events onto the queue
    /// It will not return any indication if the events were successfully executed or not.
    /// Events will be run in replay mode.
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}
