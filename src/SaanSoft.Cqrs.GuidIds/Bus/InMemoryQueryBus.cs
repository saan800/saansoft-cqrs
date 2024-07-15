using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Core.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, IIdGenerator idGenerator) :
    InMemoryQueryBus<Guid>(serviceProvider, idGenerator),
    IQueryBus,
    IQuerySubscriptionBus
{
    protected override IQuerySubscriptionBus<Guid> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IQuerySubscriptionBus>();
}
