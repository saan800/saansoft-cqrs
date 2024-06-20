using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IEventSubscriber<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Run an event from the queue
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>;
}
