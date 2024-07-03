using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, IIdGenerator idGenerator, ILogger logger) :
    InMemoryEventBus<Guid>(serviceProvider, idGenerator, logger),
    IEventBus,
    IEventSubscriptionBus;
