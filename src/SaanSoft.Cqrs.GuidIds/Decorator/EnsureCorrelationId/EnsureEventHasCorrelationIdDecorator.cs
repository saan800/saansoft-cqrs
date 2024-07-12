using SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureCorrelationId;

public class EnsureEventHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IEventBus next) :
    EnsureEventHasCorrelationIdDecorator<Guid>(providers, next),
    IEventBusDecorator
{
    public EnsureEventHasCorrelationIdDecorator(IEventBus next) : this([], next)
    {
    }
}
