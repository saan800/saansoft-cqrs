using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandPublisherDecorator(ICommandPublisherRepository<Guid> repository, ICommandBus next)
    : StoreCommandPublisherDecorator<Guid>(repository, next),
      ICommandBusDecorator;
