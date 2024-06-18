using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task ExecuteAsync_should_store_publisher_details()
    {
        var commandPublisher = A.Fake<ICommandPublisher<Guid>>();
        var store = A.Fake<ICommandPublisherStore>();

        var sut = new StoreCommandPublisherDecorator(store, commandPublisher);
        await sut.ExecuteAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyCommand).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_multiple_decorators_should_store_publisher_details()
    {
        var commandPublisher = A.Fake<ICommandPublisher<Guid>>();
        var store = A.Fake<ICommandPublisherStore>();

        var sut = new StoreCommandPublisherDecorator(store, commandPublisher);
        var wrappedInDecorator = new WrapperCommandPublisher(sut);

        await wrappedInDecorator.ExecuteAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyCommand).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_should_store_publisher_details()
    {
        var commandPublisher = A.Fake<ICommandPublisher<Guid>>();
        var store = A.Fake<ICommandPublisherStore>();

        var sut = new StoreCommandPublisherDecorator(store, commandPublisher);
        await sut.QueueAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyCommand).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_multiple_decorators_should_store_publisher_details()
    {
        var commandPublisher = A.Fake<ICommandPublisher<Guid>>();
        var store = A.Fake<ICommandPublisherStore>();

        var sut = new StoreCommandPublisherDecorator(store, commandPublisher);
        var wrappedInDecorator = new WrapperCommandPublisher(sut);

        await wrappedInDecorator.QueueAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyCommand).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    private class WrapperCommandPublisher(ICommandPublisher<Guid> next) : ICommandPublisher<Guid>
    {
        public Task<CommandResponse> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<Guid>
            => next.ExecuteAsync(command, cancellationToken);

        public Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<Guid>
            => next.QueueAsync(command, cancellationToken);
    }
}
