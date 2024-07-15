using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreQueryDecorator(IQueryRepository repository, IQueryBus next)
    : StoreQueryDecorator<Guid>(repository, next),
      IQueryBus;
