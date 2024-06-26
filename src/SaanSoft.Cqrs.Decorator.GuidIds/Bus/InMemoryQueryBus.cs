namespace SaanSoft.Cqrs.Decorator.GuidIds.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryQueryBus<Guid>(serviceProvider, logger);
