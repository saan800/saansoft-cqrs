using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryHandlerDecorator(IQueryHandlerRepository repository, IQuerySubscriptionBus next)
    : StoreQueryHandlerDecorator<Guid>(repository, next),
      IQuerySubscriptionBus;
