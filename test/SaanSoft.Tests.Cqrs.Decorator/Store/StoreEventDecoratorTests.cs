namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventDecoratorTests : EventBusDecoratorTestSetup
{
    protected StoreEventDecoratorTests()
    {
        _repository = A.Fake<IEventRepository>();
    }

    private readonly IEventRepository _repository;

    protected override IEventBusDecorator SutPublisherDecorator =>
        new StoreEventDecorator(_repository, InMemoryEventBus);

    public class QueueAsyncTests : StoreEventDecoratorTests
    {
        [Fact]
        public async Task Should_store_message_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.InsertAsync(A<MyEvent>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task IsReplay_should_NOT_store_message_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyEvent(Guid.NewGuid()) { IsReplay = true });

            A.CallTo(() => _repository.InsertAsync(A<MyEvent>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

    public class QueueManyAsyncTests : StoreEventDecoratorTests
    {
        [Fact]
        public async Task Should_store_message_details()
        {
            await SutPublisherDecorator.QueueManyAsync([
                new MyEvent(Guid.NewGuid()),
                new MyEvent(Guid.NewGuid())
            ]);

            A.CallTo(() => _repository.InsertAsync(A<MyEvent>._, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task IsReplay_should_NOT_store_message_details()
        {
            await SutPublisherDecorator.QueueManyAsync([new MyEvent(Guid.NewGuid()) { IsReplay = true }]);

            A.CallTo(() => _repository.InsertAsync(A<MyEvent>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

}
