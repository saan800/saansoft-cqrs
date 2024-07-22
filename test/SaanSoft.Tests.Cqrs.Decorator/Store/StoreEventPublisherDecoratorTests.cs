namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventPublisherDecoratorTests : EventBusDecoratorTestSetup
{
    protected override IEventBus SutPublisherDecorator =>
        new StoreEventPublisherDecorator(InMemoryEventBus);

    public class QueueAsyncTests : StoreEventPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(QueueAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            var evt = new MyEvent(Guid.NewGuid());
            await SutPublisherDecorator.QueueAsync(evt);

            var publisher = evt.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }

        [Fact]
        public async Task Multiple_decorators_should_store_publisher_details()
        {
            var evt = new MyEvent(Guid.NewGuid());
            var wrappedInDecorator = new WrapperEventBusDecorator(SutPublisherDecorator);

            await wrappedInDecorator.QueueAsync(evt);

            var publisher = evt.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
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

            var publisher1 = event1.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher1.Should().Be(ExpectedType.FullName);

            var publisher2 = event2.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher2.Should().Be(ExpectedType.FullName);
        }
    }

    private class WrapperEventBusDecorator(IEventBus next) : IEventBus
    {
        public Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent
            => next.QueueAsync(evt, cancellationToken);

        public Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent
            => next.QueueManyAsync(events, cancellationToken);
    }
}
