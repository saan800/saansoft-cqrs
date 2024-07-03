using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandDecorator(ICommandRepository repository, ICommandBus next)
    : StoreCommandDecorator<Guid>(repository, next),
      ICommandBusDecorator;
