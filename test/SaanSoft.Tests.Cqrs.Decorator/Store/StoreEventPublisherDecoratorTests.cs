namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventPublisherDecoratorTests : EventBusDecoratorTestSetup
{
    protected StoreEventPublisherDecoratorTests()
    {
        _repository = A.Fake<IEventPublisherRepository<Guid>>();
    }

    private readonly IEventPublisherRepository<Guid> _repository;
    protected override IEventBusDecorator<Guid> SutPublisherDecorator =>
        new StoreEventPublisherDecorator(_repository, InMemoryEventBus);

    public class QueueAsyncTests : StoreEventPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(QueueAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyEvent>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task Multiple_decorators_should_store_publisher_details()
        {
            var wrappedInDecorator = new WrapperEventBusDecorator(SutPublisherDecorator);

            await wrappedInDecorator.QueueAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyEvent>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }
    }

    public class QueueManyAsyncTests : StoreEventPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(QueueManyAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            var event1 = new MyEvent(Guid.NewGuid());
            var event2 = new MyEvent(Guid.NewGuid());

            await SutPublisherDecorator.QueueManyAsync([event1, event2]);

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyEvent>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task No_events_in_list_Should_store_publisher_details()
        {
            await SutPublisherDecorator.QueueManyAsync(new List<MyEvent>());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyEvent>._, ExpectedType, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }


    private class WrapperEventBusDecorator(IEventBus<Guid> next) : IEventBus<Guid>
    {
        public Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
            => next.QueueAsync(evt, cancellationToken);

        public Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
            => next.QueueManyAsync(events, cancellationToken);
    }
}
