namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class QuerySubscriptionBusDecoratorTestSetup : TestSetup
{
    protected QuerySubscriptionBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();
        ServiceCollection.AddScoped<IQueryHandler<AnotherQuery, MyQueryResponse>, QueryHandler>();
    }

    protected abstract IQuerySubscriptionBus SutSubscriptionBusDecorator { get; }
}
