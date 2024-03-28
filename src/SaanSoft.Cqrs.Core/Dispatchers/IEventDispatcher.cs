using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Dispatchers;

public interface IEventDispatcher
{
    /// <summary>
    /// Put the event onto the queue
    /// It will not have any indication if the event was successfully executed or not
    /// </summary>
    /// <param name="evt"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TEvent>(TEvent evt) where TEvent : IEvent;

    /// <summary>
    /// Put the events onto the queue
    /// It will not have any indication if the event was successfully executed or not
    /// </summary>
    /// <param name="events"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent;
}
