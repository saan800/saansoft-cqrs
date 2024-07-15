using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class EventSubscriptionBusTestSetup : TestSetup
{
    protected EventSubscriptionBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseEventHandler<MyEvent>, EventsHandler>();
        ServiceCollection.AddScoped<IBaseEventHandler<AnotherEvent>, EventsHandler>();
    }

    protected abstract IEventSubscriptionBus SutSubscriptionBus { get; }
}
