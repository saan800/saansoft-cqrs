using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task ExecuteAsync_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        await sut.ExecuteAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_for_IsReplay_command_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        await sut.ExecuteAsync(new MyCommand { IsReplay = true });

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_multiple_decorators_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        var wrappedInDecorator = new WrapperCommandBusDecorator(sut);

        await wrappedInDecorator.ExecuteAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        await sut.QueueAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_for_IsReplay_command_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        await sut.QueueAsync(new MyCommand { IsReplay = true });

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_multiple_decorators_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        var store = A.Fake<ICommandPublisherRepository<Guid>>();

        var sut = new StoreCommandPublisherDecorator(store, InMemoryCommandBus);
        var wrappedInDecorator = new WrapperCommandBusDecorator(sut);

        await wrappedInDecorator.QueueAsync(new MyCommand());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyCommand>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    private class WrapperCommandBusDecorator(ICommandBus<Guid> next) : ICommandBus<Guid>
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
