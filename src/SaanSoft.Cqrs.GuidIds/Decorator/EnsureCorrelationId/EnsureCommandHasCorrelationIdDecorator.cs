using SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

namespace SaanSoft.Cqrs.GuidIds.Decorator.EnsureCorrelationId;

public class EnsureCommandHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, ICommandBus next) :
    EnsureCommandHasCorrelationIdDecorator<Guid>(providers, next),
    ICommandBusDecorator
{
    public EnsureCommandHasCorrelationIdDecorator(ICommandBus next) : this([], next)
    {
    }
}
