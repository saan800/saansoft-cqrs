using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreEventDecorator(IEventRepository repository, IEventBus next)
    : StoreEventDecorator<Guid>(repository, next);

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public class StoreEventDecorator<TEntityKey>(IEventRepository<TEntityKey> repository, IEventBus next)
    : StoreEventDecorator<Guid, TEntityKey>(repository, next),
      IEventBusDecorator
    where TEntityKey : struct;
