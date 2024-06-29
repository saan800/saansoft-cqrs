using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryPublisherDecorator(IQueryPublisherRepository<Guid> repository, IQueryBus<Guid> next)
    : StoreQueryPublisherDecorator<Guid>(repository, next);
