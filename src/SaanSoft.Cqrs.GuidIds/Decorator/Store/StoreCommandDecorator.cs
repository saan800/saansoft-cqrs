using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

public class StoreCommandDecorator(ICommandRepository<Guid> repository, ICommandBus<Guid> next)
    : StoreCommandDecorator<Guid>(repository, next);
