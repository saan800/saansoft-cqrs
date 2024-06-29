using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryHandlerDecorator(IQueryHandlerRepository<Guid> repository, IQuerySubscriptionBus<Guid> next)
    : StoreQueryHandlerDecorator<Guid>(repository, next);
