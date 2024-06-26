namespace SaanSoft.Cqrs.Decorator.GuidIds.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, IIdGenerator<Guid> idGenerator, ILogger logger)
    : InMemoryEventBus<Guid>(serviceProvider, idGenerator, logger);
