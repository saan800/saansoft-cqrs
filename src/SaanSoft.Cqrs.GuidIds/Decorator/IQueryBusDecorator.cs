using SaanSoft.Cqrs.Decorator;

namespace SaanSoft.Cqrs.GuidIds.Decorator;

public interface IQueryBusDecorator : IQueryBusDecorator<Guid>, IQueryBus
{
}
