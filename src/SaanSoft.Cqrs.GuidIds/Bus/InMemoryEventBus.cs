using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, IIdGenerator idGenerator) :
    InMemoryEventBus<Guid>(serviceProvider, idGenerator),
    IEventBus,
    IEventSubscriptionBus
{
    protected override IEventSubscriptionBus GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IEventSubscriptionBus>();
}
