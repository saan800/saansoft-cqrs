using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryEventBusTests : TestSetup
{
    public class Constructor : InMemoryEventBusTests
    {
        [Fact]
        public void Cant_create_with_null_serviceProvider()
        {
            Action act = () => new InMemoryEventBus(null);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "serviceProvider");
        }
    }

    public class QueueAsync : InMemoryEventBusTests
    {
        [Fact]
        public async Task QueueAsync_single_handler_exists_in_serviceProvider()
        {
            var eventHandler = A.Fake<IEventHandler<MyEvent>>();

            ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);

            await InMemoryEventBus.QueueAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => eventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        }

        /// <summary>
        /// Unlike commands and queries, having multiple event handlers is fine,
        /// and both event handlers should be run
        /// </summary>
        [Fact]
        public async Task QueueAsync_multiple_handlers_exists_in_serviceProvider()
        {
            var eventHandler = A.Fake<IEventHandler<MyEvent>>();
            var anotherEventHandler = A.Fake<IEventHandler<MyEvent>>();

            ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);
            ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => anotherEventHandler);

            await InMemoryEventBus.QueueAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => eventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => anotherEventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        }

        /// <summary>
        /// Unlike commands and queries, having no event handlers is fine, it just does nothing.
        /// </summary>
        [Fact]
        public async Task QueueAsync_no_handler_in_serviceProvider_should_do_nothing()
        {
            await InMemoryEventBus.QueueAsync(new MyEvent(Guid.NewGuid()));

            Assert.True(true);
        }
    }

    public class QueueManyAsync : InMemoryEventBusTests
    {
        [Fact]
        public async Task QueueManyAsync_executes_multiple_events_of_same_type()
        {
            var eventHandler = A.Fake<IEventHandler<MyEvent>>();
            ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);

            var event1 = new MyEvent(Guid.NewGuid());
            var event2 = new MyEvent(Guid.NewGuid());

            await InMemoryEventBus.QueueManyAsync([event1, event2]);

            A.CallTo(() => eventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened(2, Times.Exactly);
        }
    }
}
