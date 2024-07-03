using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryDecorator(IQueryRepository<Guid> repository, IQueryBus<Guid> next)
    : StoreQueryDecorator<Guid>(repository, next);
