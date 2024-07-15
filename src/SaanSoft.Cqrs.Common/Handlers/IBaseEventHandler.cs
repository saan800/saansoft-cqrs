using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Common.Handlers;

public interface IBaseEventHandler<in TEvent> where TEvent : IBaseEvent
{
    /// <summary>
    /// Handle the event. Often includes updates to the DB state.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(TEvent evt, CancellationToken cancellationToken = default);
}
