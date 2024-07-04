namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class EventSubscriptionBusDecoratorTestSetup : TestSetup
{
    protected EventSubscriptionBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();
        ServiceCollection.AddScoped<IEventHandler<AnotherEvent>, EventsHandler>();
    }

    protected abstract IEventSubscriptionBusDecorator SutSubscriptionBusDecorator { get; }
}
