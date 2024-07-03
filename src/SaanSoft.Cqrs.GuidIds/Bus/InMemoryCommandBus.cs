using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider, IIdGenerator idGenerator, ILogger logger) :
    InMemoryCommandBus<Guid>(serviceProvider, idGenerator, logger),
    ICommandBus,
    ICommandSubscriptionBus;
