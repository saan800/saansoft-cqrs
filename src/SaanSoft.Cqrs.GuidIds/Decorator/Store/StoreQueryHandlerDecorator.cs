using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryHandlerDecorator(IQueryHandlerRepository<Guid> repository, IQuerySubscriptionBus next)
    : StoreQueryHandlerDecorator<Guid>(repository, next),
      IQuerySubscriptionBusDecorator;
