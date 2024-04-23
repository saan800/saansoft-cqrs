using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IEventPublisher<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Put the event onto the queue
    /// It will not return any indication if the event was successfully executed or not.
    /// Events will be run in replay mode.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>;

    /// <summary>
    /// Put the events onto the queue
    /// It will not return any indication if the events were successfully executed or not.
    /// Events will be run in replay mode.
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>;
}
