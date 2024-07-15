using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class QuerySubscriptionBusTestSetup : TestSetup
{
    protected QuerySubscriptionBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();
        ServiceCollection.AddScoped<IBaseQueryHandler<AnotherQuery, MyQueryResponse>, QueryHandler>();
    }

    protected abstract IQuerySubscriptionBus SutSubscriptionBus { get; }
}
