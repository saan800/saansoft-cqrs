using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Handlers;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent evt);
}
