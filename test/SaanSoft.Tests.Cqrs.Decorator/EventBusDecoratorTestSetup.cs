using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class EventBusTestSetup : TestSetup
{
    protected EventBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseEventHandler<MyEvent>, EventsHandler>();
        ServiceCollection.AddScoped<IBaseEventHandler<AnotherEvent>, EventsHandler>();
    }

    protected abstract IEventBus SutPublisherDecorator { get; }
}

