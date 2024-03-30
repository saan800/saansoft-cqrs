using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IEventHandler
{
    Task HandleAsync<TEvent>(TEvent evt) where TEvent : IEvent;
}
