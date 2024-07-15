namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandDecoratorTests : CommandBusTestSetup
{
    protected StoreCommandDecoratorTests()
    {
        _repository = A.Fake<ICommandRepository>();
    }

    private readonly ICommandRepository _repository;

    protected override ICommandBus SutPublisherDecorator =>
        new StoreCommandDecorator(_repository, InMemoryCommandBus);

    public class ExecuteAsyncTests : StoreCommandDecoratorTests
    {
        [Fact]
        public async Task Should_store_message_details()
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommand());

            A.CallTo(() => _repository.InsertAsync(A<MyCommand>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task IsReplay_should_NOT_store_message_details()
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommand { IsReplay = true });

            A.CallTo(() => _repository.InsertAsync(A<MyCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

    public class ExecuteAsyncWithResponseTests : StoreCommandDecoratorTests
    {
        [Theory]
        [AutoFakeData]
        public async Task Should_store_message_details(string message)
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommandWithResponse { Message = message });

            A.CallTo(() => _repository.InsertAsync(A<MyCommandWithResponse>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Theory]
        [AutoFakeData]
        public async Task IsReplay_should_NOT_store_message_details(string message)
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommandWithResponse { Message = message, IsReplay = true });

            A.CallTo(() => _repository.InsertAsync(A<MyCommandWithResponse>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

    public class QueueAsyncTests : StoreCommandDecoratorTests
    {
        [Fact]
        public async Task Should_store_message_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyCommand());

            A.CallTo(() => _repository.InsertAsync(A<MyCommand>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task IsReplay_should_NOT_store_message_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyCommand { IsReplay = true });

            A.CallTo(() => _repository.InsertAsync(A<MyCommand>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }
}
