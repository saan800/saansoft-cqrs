using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandHandlerDecorator(ICommandHandlerRepository<Guid> repository, ICommandSubscriptionBus<Guid> next)
    : StoreCommandHandlerDecorator<Guid>(repository, next);
