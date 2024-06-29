namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider, IIdGenerator<Guid> idGenerator, ILogger logger)
    : InMemoryCommandBus<Guid>(serviceProvider, idGenerator, logger);
