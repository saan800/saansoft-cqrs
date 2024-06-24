using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandHandlerDecoratorTests : TestSetup
{
    [Fact]
    public async Task RunAsync_should_store_single_handler_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriptionBus = new InMemoryCommandBus(GetServiceProvider(), Logger);
        var store = A.Fake<ICommandHandlerRepository<Guid>>();

        var sut = new StoreCommandHandlerDecorator(store, commandSubscriptionBus);
        await sut.RunAsync(new MyCommand());

        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, typeof(CommandHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_handler_details()
    {
        var commandSubscriptionBus = new InMemoryCommandBus(GetServiceProvider(), Logger);
        var store = A.Fake<ICommandHandlerRepository<Guid>>();

        var sut = new StoreCommandHandlerDecorator(store, commandSubscriptionBus);
        await sut.Invoking(y => y.RunAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("No handler for type"));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_multiple_handlers_details()
    {
        var handler1 = A.Fake<ICommandHandler<MyCommand>>();
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriptionBus = new InMemoryCommandBus(GetServiceProvider(), Logger);
        var store = A.Fake<ICommandHandlerRepository<Guid>>();

        var sut = new StoreCommandHandlerDecorator(store, commandSubscriptionBus);

        await sut.Invoking(y => y.RunAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("Only one handler for type"));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_store_handlers_details_when_next_throws_exception()
    {
        var handler = A.Fake<ICommandHandler<MyCommand>>();
        A.CallTo(() => handler.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

        var commandSubscriptionBus = new InMemoryCommandBus(GetServiceProvider(), Logger);
        var store = A.Fake<ICommandHandlerRepository<Guid>>();

        var sut = new StoreCommandHandlerDecorator(store, commandSubscriptionBus);

        await sut.Invoking(y => y.RunAsync(new MyCommand()))
            .Should().ThrowAsync<Exception>()
            .Where(x => x.Message.StartsWith("it went wrong"));

        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}

