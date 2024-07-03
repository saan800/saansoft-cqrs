using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public interface IEventRepository : IEventRepository<Guid>
{
}

public interface IEventRepository<TEntityKey> :
    IEventRepository<Guid, TEntityKey>
    where TEntityKey : struct
{
}
