namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class QueryBusDecoratorTestSetup : TestSetup
{
    protected QueryBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();
        ServiceCollection.AddScoped<IQueryHandler<AnotherQuery, MyQueryResponse>, QueryHandler>();
    }

    protected abstract IQueryBusDecorator<Guid> SutPublisherDecorator { get; }
}

