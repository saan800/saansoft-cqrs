namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandPublisherDecoratorTests : CommandBusDecoratorTestSetup
{
    protected StoreCommandPublisherDecoratorTests()
    {
        _repository = A.Fake<ICommandPublisherRepository<Guid>>();
    }

    private readonly ICommandPublisherRepository<Guid> _repository;

    protected override ICommandBusDecorator SutPublisherDecorator =>
        new StoreCommandPublisherDecorator(_repository, InMemoryCommandBus);

    public class ExecuteAsyncTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(ExecuteAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommand());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task IsReplay_command_should_store_publisher_details()
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommand { IsReplay = true });

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task Multiple_decorators_should_store_publisher_details()
        {
            var wrappedInDecorator = new WrapperCommandBusDecorator(SutPublisherDecorator);
            await wrappedInDecorator.ExecuteAsync(new MyCommand());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }
    }

    public class ExecuteAsyncWithResponseTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(ExecuteAsyncWithResponseTests);

        [Theory]
        [AutoFakeData]
        public async Task Should_store_publisher_details(string message)
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommandWithResponse { Message = message });

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommandWithResponse>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Theory]
        [AutoFakeData]
        public async Task IsReplay_command_should_store_publisher_details(string message)
        {
            await SutPublisherDecorator.ExecuteAsync(new MyCommandWithResponse { Message = message, IsReplay = true });

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommandWithResponse>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Theory]
        [AutoFakeData]
        public async Task Multiple_decorators_should_store_publisher_details(string message)
        {
            var wrappedInDecorator = new WrapperCommandBusDecorator(SutPublisherDecorator);
            await wrappedInDecorator.ExecuteAsync(new MyCommandWithResponse { Message = message });

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommandWithResponse>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }
    }

    public class QueueAsyncTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(QueueAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyCommand());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task IsReplay_command_should_store_publisher_details()
        {
            await SutPublisherDecorator.QueueAsync(new MyCommand { IsReplay = true });

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task QueueAsync_multiple_decorators_should_store_publisher_details()
        {
            var wrappedInDecorator = new WrapperCommandBusDecorator(SutPublisherDecorator);
            await wrappedInDecorator.QueueAsync(new MyCommand());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyCommand>._, ExpectedType, A<CancellationToken>._))
                .MustHaveHappened();
        }
    }

    private class WrapperCommandBusDecorator(ICommandBus next) : ICommandBus
    {
        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<Guid>
            => next.ExecuteAsync(command, cancellationToken);

        public Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TCommand, TResponse>, ICommand<Guid, TCommand, TResponse>
            => next.ExecuteAsync(command, cancellationToken);

        public Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<Guid>
            => next.QueueAsync(command, cancellationToken);
    }
}
