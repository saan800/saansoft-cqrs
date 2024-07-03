namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class EventBusDecoratorTestSetup : TestSetup
{
    protected EventBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();
        ServiceCollection.AddScoped<IEventHandler<AnotherEvent>, EventsHandler>();
    }

    protected abstract IEventBusDecorator SutPublisherDecorator { get; }
}

