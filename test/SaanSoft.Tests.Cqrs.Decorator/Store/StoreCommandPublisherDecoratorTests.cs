using SaanSoft.Cqrs.Common.Messages;
using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandPublisherDecoratorTests : CommandBusTestSetup
{
    protected override ICommandBus SutPublisherDecorator =>
        new StoreCommandPublisherDecorator(InMemoryCommandBus);

    public class ExecuteAsyncTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(ExecuteAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.ExecuteAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }

        [Fact]
        public async Task Multiple_decorators_should_store_publisher_details()
        {
            var command = new MyCommand();
            var wrappedInDecorator = new WrapperCommandBus(SutPublisherDecorator);
            await wrappedInDecorator.ExecuteAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }
    }

    public class ExecuteAsyncWithResponseTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(ExecuteAsyncWithResponseTests);

        [Theory]
        [AutoFakeData]
        public async Task Should_store_publisher_details(string message)
        {
            var command = new MyCommandWithResponse { Message = message };
            await SutPublisherDecorator.ExecuteAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }

        [Theory]
        [AutoFakeData]
        public async Task Multiple_decorators_should_store_publisher_details(string message)
        {
            var command = new MyCommandWithResponse { Message = message };
            var wrappedInDecorator = new WrapperCommandBus(SutPublisherDecorator);
            await wrappedInDecorator.ExecuteAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }
    }

    public class QueueAsyncTests : StoreCommandPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(QueueAsyncTests);

        [Fact]
        public async Task Should_store_publisher_details()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.QueueAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }

        [Fact]
        public async Task QueueAsync_multiple_decorators_should_store_publisher_details()
        {
            var command = new MyCommand();
            var wrappedInDecorator = new WrapperCommandBus(SutPublisherDecorator);
            await wrappedInDecorator.QueueAsync(command);

            var publisher = command.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }
    }

    private class WrapperCommandBus(ICommandBus next) : ICommandBus
    {
        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, IBaseCommand<Guid>
            => next.ExecuteAsync(command, cancellationToken);

        public Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
            where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<Guid, TCommand, TResponse>
            => next.ExecuteAsync(command, cancellationToken);

        public Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, IBaseCommand<Guid>
            => next.QueueAsync(command, cancellationToken);
    }
}
