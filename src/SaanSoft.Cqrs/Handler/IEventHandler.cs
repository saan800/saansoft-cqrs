using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handle the event. Often includes updates to the DB state.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(TEvent evt, CancellationToken cancellationToken = default);
}
