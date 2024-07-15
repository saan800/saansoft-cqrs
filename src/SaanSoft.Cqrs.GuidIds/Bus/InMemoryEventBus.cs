using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Core.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, IIdGenerator idGenerator) :
    BaseInMemoryEventBus<Guid>(serviceProvider, idGenerator),
    IEventBus,
    IEventSubscriptionBus
{
    protected override IEventSubscriptionBus GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IEventSubscriptionBus>();
}
