namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId.GuidIds;

public class EnsureEventHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IEventBus next) :
    EnsureEventHasCorrelationIdDecorator<Guid>(providers, next),
    IEventBus
{
    public EnsureEventHasCorrelationIdDecorator(IEventBus next) : this([], next)
    {
    }
}
