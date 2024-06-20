using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventSubscriberDecoratorTests : TestSetup
{
    #region RunAsync

    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriber = A.Fake<IEventSubscriber<Guid>>();
        var store = A.Fake<IEventSubscriberStore>();

        var sut = new StoreEventSubscriberDecorator(GetServiceProvider(), store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyEvent).FullName!, A<IEnumerable<string>>.That.Contains(typeof(EventsHandler).FullName!), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var eventSubscriber = A.Fake<IEventSubscriber<Guid>>();
        var store = A.Fake<IEventSubscriberStore>();

        var sut = new StoreEventSubscriberDecorator(GetServiceProvider(), store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyEvent).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<IEventHandler<MyEvent>>();
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler1);
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriber = A.Fake<IEventSubscriber<Guid>>();
        var store = A.Fake<IEventSubscriberStore>();

        var sut = new StoreEventSubscriberDecorator(GetServiceProvider(), store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyEvent).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyEvent).FullName!, A<IEnumerable<string>>.That.Contains(typeof(EventsHandler).FullName!), A<CancellationToken>._)).MustHaveHappened();
    }

    #endregion

}

