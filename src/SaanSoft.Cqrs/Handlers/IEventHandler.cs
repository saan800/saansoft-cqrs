namespace SaanSoft.Cqrs.Handlers;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handle the event. Often includes updates to the DB state.
    /// </summary>
    Task HandleAsync(TEvent evt, CancellationToken ct = default);
}
