using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;
using SaanSoft.Tests.Cqrs.Common.TestSubscribers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandSubscriberDecoratorTests : TestSetup
{
    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriber = new TestCommandSubscriber(GetServiceProvider());
        var store = A.Fake<ICommandSubscriberStore<Guid>>();

        var sut = new StoreCommandSubscriberDecorator(store, commandSubscriber);
        await sut.RunAsync(new MyCommand());

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, typeof(CommandHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var commandSubscriber = new TestCommandSubscriber(GetServiceProvider());
        var store = A.Fake<ICommandSubscriberStore<Guid>>();

        var sut = new StoreCommandSubscriberDecorator(store, commandSubscriber);
        await sut.Invoking(y => y.RunAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("No handler for type"));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<ICommandHandler<MyCommand>>();
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriber = new TestCommandSubscriber(GetServiceProvider());
        var store = A.Fake<ICommandSubscriberStore<Guid>>();

        var sut = new StoreCommandSubscriberDecorator(store, commandSubscriber);

        await sut.Invoking(y => y.RunAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("Only one handler for type"));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }
}

