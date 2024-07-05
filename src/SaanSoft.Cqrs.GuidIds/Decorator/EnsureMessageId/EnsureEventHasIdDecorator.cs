using SaanSoft.Cqrs.Decorator.EnsureMessageId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

public class EnsureEventHasIdDecorator(IIdGenerator idGenerator, IEventBus next) :
    EnsureEventHasIdDecorator<Guid>(idGenerator, next),
    IEventBusDecorator;