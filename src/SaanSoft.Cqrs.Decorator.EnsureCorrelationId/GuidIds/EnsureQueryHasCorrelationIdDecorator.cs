namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId.GuidIds;

public class EnsureQueryHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IQueryBus next) :
    EnsureQueryHasCorrelationIdDecorator<Guid>(providers, next),
    IQueryBusDecorator
{
    public EnsureQueryHasCorrelationIdDecorator(IQueryBus next) : this([], next)
    {
    }
}
