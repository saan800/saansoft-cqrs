namespace SaanSoft.Cqrs.Decorator.GuidIds.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, IIdGenerator<Guid> idGenerator, ILogger logger)
    : InMemoryQueryBus<Guid>(serviceProvider, idGenerator, logger);
