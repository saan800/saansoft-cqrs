using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreEventHandlerDecorator(IEventHandlerRepository repository, IEventSubscriptionBus next)
    : StoreEventHandlerDecorator<Guid>(repository, next),
      IEventSubscriptionBus;
