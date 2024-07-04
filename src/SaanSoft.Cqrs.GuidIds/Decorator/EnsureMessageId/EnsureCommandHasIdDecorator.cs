using SaanSoft.Cqrs.Decorator.EnsureMessageId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

public class EnsureCommandHasIdDecorator(IIdGenerator idGenerator, ICommandBus next) :
    EnsureCommandHasIdDecorator<Guid>(idGenerator, next),
    ICommandBusDecorator;
