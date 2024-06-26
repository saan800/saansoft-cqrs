namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventHandlerDecoratorTests : TestSetup
{
    #region RunAsync

    [Fact]
    public async Task RunAsync_should_store_single_handler_details()
    {
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriptionBus = new InMemoryEventBus(GetServiceProvider(), Logger);
        var store = A.Fake<IEventHandlerRepository<Guid>>();

        var sut = new StoreEventHandlerDecorator(store, eventSubscriptionBus);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, typeof(EventsHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_handler_details()
    {
        var eventSubscriptionBus = new InMemoryEventBus(GetServiceProvider(), Logger);
        var store = A.Fake<IEventHandlerRepository<Guid>>();

        var sut = new StoreEventHandlerDecorator(store, eventSubscriptionBus);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_store_multiple_handlers_details()
    {
        var handler1 = A.Fake<IEventHandler<MyEvent>>();
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler1);
        ServiceCollection.AddScoped<IEventHandler<MyEvent>, EventsHandler>();

        var eventSubscriptionBus = new InMemoryEventBus(GetServiceProvider(), Logger);
        var store = A.Fake<IEventHandlerRepository<Guid>>();

        var sut = new StoreEventHandlerDecorator(store, eventSubscriptionBus);
        await sut.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_store_handlers_details_when_next_throws_exception()
    {
        var handler = A.Fake<IEventHandler<MyEvent>>();
        A.CallTo(() => handler.HandleAsync(A<MyEvent>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler);

        var eventSubscriptionBus = new InMemoryEventBus(GetServiceProvider(), Logger);
        var store = A.Fake<IEventHandlerRepository<Guid>>();

        var sut = new StoreEventHandlerDecorator(store, eventSubscriptionBus);

        await sut.Invoking(y => y.RunAsync(new MyEvent(Guid.NewGuid())))
            .Should().ThrowAsync<Exception>()
            .Where(x => x.Message.StartsWith("it went wrong"));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyEvent>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    #endregion

}

