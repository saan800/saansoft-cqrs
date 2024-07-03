using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreEventDecorator(IEventRepository<Guid, Guid> repository, IEventBus<Guid> next)
    : StoreEventDecorator<Guid, Guid>(repository, next);

public class StoreEventDecorator<TEntityKey>(IEventRepository<Guid, TEntityKey> repository, IEventBus<Guid> next)
    : StoreEventDecorator<Guid, TEntityKey>(repository, next)
    where TEntityKey : struct;
