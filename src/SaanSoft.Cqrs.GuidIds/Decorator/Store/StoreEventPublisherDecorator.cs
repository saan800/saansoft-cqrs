using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreEventPublisherDecorator(IEventPublisherRepository<Guid> repository, IEventBus<Guid> next)
    : StoreEventPublisherDecorator<Guid>(repository, next);
