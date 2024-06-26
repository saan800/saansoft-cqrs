namespace SaanSoft.Cqrs.Decorator.GuidIds.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryEventBus<Guid>(serviceProvider, logger);
