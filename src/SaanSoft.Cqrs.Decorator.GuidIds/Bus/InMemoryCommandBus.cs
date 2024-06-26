namespace SaanSoft.Cqrs.Decorator.GuidIds.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryCommandBus<Guid>(serviceProvider, logger);
