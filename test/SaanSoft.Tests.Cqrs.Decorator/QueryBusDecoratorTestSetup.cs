using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class QueryBusTestSetup : TestSetup
{
    protected QueryBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();
        ServiceCollection.AddScoped<IBaseQueryHandler<AnotherQuery, MyQueryResponse>, QueryHandler>();
    }

    protected abstract IQueryBus SutPublisherDecorator { get; }
}

