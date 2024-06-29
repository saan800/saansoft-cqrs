using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandPublisherDecorator(ICommandPublisherRepository<Guid> repository, ICommandBus<Guid> next)
    : StoreCommandPublisherDecorator<Guid>(repository, next);
