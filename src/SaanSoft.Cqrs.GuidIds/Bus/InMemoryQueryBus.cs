using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Core.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, IIdGenerator idGenerator) :
    BaseInMemoryQueryBus<Guid>(serviceProvider, idGenerator),
    IQueryBus,
    IQuerySubscriptionBus
{
    protected override IBaseQuerySubscriptionBus<Guid> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IQuerySubscriptionBus>();
}
