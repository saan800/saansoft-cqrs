using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public interface IEventHandlerRepository : IEventHandlerRepository<Guid>
{
}