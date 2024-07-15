using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandHandlerDecorator(ICommandHandlerRepository repository, ICommandSubscriptionBus next)
    : StoreCommandHandlerDecorator<Guid>(repository, next),
      ICommandSubscriptionBus;
