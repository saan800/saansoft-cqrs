using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task QueueAsync_should_store_publisher_details()
    {
        var eventPublisher = A.Fake<IEventPublisher<Guid>>();
        var store = A.Fake<IEventPublisherStore<Guid>>();

        var sut = new StoreEventPublisherDecorator(store, eventPublisher);
        await sut.QueueAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertPublisherAsync(A<MyEvent>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_multiple_decorators_should_store_publisher_details()
    {
        var eventPublisher = A.Fake<IEventPublisher<Guid>>();
        var store = A.Fake<IEventPublisherStore<Guid>>();

        var sut = new StoreEventPublisherDecorator(store, eventPublisher);
        var wrappedInDecorator = new WrapperEventPublisher(sut);

        await wrappedInDecorator.QueueAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => store.UpsertPublisherAsync(A<MyEvent>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueManyAsync_should_store_publisher_details()
    {
        var eventPublisher = A.Fake<IEventPublisher<Guid>>();
        var store = A.Fake<IEventPublisherStore<Guid>>();
        var event1 = new MyEvent(Guid.NewGuid());
        var event2 = new MyEvent(Guid.NewGuid());

        var sut = new StoreEventPublisherDecorator(store, eventPublisher);
        await sut.QueueManyAsync([event1, event2]);

        A.CallTo(() => store.UpsertPublisherAsync(A<MyEvent>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened(1, Times.Exactly);
    }

    private class WrapperEventPublisher(IEventPublisher<Guid> next) : IEventPublisher<Guid>
    {
        public Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
            => next.QueueAsync(evt, cancellationToken);

        public Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
            => next.QueueManyAsync(events, cancellationToken);
    }
}
