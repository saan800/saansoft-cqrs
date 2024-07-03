using SaanSoft.Cqrs.Decorator;

namespace SaanSoft.Cqrs.GuidIds.Decorator;

public interface ICommandBusDecorator : ICommandBusDecorator<Guid>, ICommandBus
{
}
