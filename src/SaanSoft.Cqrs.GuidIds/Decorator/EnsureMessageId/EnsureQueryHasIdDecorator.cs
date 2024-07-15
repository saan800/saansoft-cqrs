using SaanSoft.Cqrs.Decorator.EnsureMessageId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

public class EnsureQueryHasIdDecorator(IIdGenerator idGenerator, IQueryBus next) :
    EnsureQueryHasIdDecorator<Guid>(idGenerator, next),
    IQueryBus;
