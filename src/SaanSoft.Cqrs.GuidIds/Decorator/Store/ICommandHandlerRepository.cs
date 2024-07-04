using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public interface ICommandHandlerRepository : ICommandHandlerRepository<Guid>
{
}
