using SaanSoft.Cqrs.Decorator;

namespace SaanSoft.Cqrs.GuidIds.Decorator;

public interface IEventBusDecorator : IEventBusDecorator<Guid>, IEventBus
{
}
