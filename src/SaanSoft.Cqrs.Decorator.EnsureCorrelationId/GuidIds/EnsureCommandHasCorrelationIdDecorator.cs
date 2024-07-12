namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId.GuidIds;

public class EnsureCommandHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, ICommandBus next) :
    EnsureCommandHasCorrelationIdDecorator<Guid>(providers, next),
    ICommandBusDecorator
{
    public EnsureCommandHasCorrelationIdDecorator(ICommandBus next) : this([], next)
    {
    }
}
