using SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureCorrelationId;

public class EnsureQueryHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IQueryBus next) :
    EnsureQueryHasCorrelationIdDecorator<Guid>(providers, next),
    IQueryBusDecorator
{
    public EnsureQueryHasCorrelationIdDecorator(IQueryBus next) : this([], next)
    {
    }
}
