using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IEventSubscriber<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Run an event from the queue against all handlers that subscribe to the message
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>;

    /// <summary>
    /// Run an event from the queue against a particular handler
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="handler"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task RunOneAsync<TEvent>(TEvent evt, IEventHandler<TEvent> handler, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>;

    /// <summary>
    /// Get handlers for an event, grouped and ordered by handler priority
    ///
    /// Can have 0-n handlers of an event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>()
        where TEvent : IEvent<TMessageId>;
}
