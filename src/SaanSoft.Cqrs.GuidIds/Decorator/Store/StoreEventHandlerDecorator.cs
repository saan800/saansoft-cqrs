using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreEventHandlerDecorator(IEventHandlerRepository<Guid> repository, IEventSubscriptionBus<Guid> next)
    : StoreEventHandlerDecorator<Guid>(repository, next);
