using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, IIdGenerator idGenerator, ILogger logger) :
    InMemoryQueryBus<Guid>(serviceProvider, idGenerator, logger),
    IQueryBus,
    IQuerySubscriptionBus
{
    protected override IQuerySubscriptionBus<Guid> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IQuerySubscriptionBus>();
}
