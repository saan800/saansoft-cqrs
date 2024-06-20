using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;
using SaanSoft.Tests.Cqrs.Common.TestSubscribers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventSubscriberDecoratorTests : TestSetup
{
    #region RunAsync

    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriber = new TestEventSubscriber(GetServiceProvider());
        var store = A.Fake<IEventSubscriberStore<Guid>>();

        var sut = new StoreEventSubscriberDecorator(store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, typeof(EventsHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var eventSubscriber = new TestEventSubscriber(GetServiceProvider());
        var store = A.Fake<IEventSubscriberStore<Guid>>();

        var sut = new StoreEventSubscriberDecorator(store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<IEventHandler<MyEvent>>();
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler1);
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriber = new TestEventSubscriber(GetServiceProvider());
        var store = A.Fake<IEventSubscriberStore<Guid>>();

        var sut = new StoreEventSubscriberDecorator(store, eventSubscriber);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_store_subscribers_details_when_next_throws_exception()
    {
        var handler = A.Fake<IEventHandler<MyEvent>>();
        A.CallTo(() => handler.HandleAsync(A<MyEvent>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler);

        var eventSubscriber = new TestEventSubscriber(GetServiceProvider());
        var store = A.Fake<IEventSubscriberStore<Guid>>();

        var sut = new StoreEventSubscriberDecorator(store, eventSubscriber);

        await sut.Invoking(y => y.RunAsync(new MyEvent(Guid.NewGuid())))
            .Should().ThrowAsync<Exception>()
            .Where(x => x.Message.StartsWith("it went wrong"));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    #endregion

}

