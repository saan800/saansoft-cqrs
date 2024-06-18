using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandSubscriberDecoratorTests : TestSetup
{
    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriber = A.Fake<ICommandSubscriber<Guid>>();
        var store = A.Fake<ICommandSubscriberStore>();

        var sut = new StoreCommandSubscriberDecorator(GetServiceProvider(), store, commandSubscriber);
        await sut.RunAsync(new MyCommand());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyCommand).FullName!, A<IEnumerable<string>>.That.Contains(typeof(CommandHandler).FullName!), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var commandSubscriber = A.Fake<ICommandSubscriber<Guid>>();
        var store = A.Fake<ICommandSubscriberStore>();

        var sut = new StoreCommandSubscriberDecorator(GetServiceProvider(), store, commandSubscriber);
        await sut.RunAsync(new MyCommand());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyCommand).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<ICommandHandler<MyCommand>>();
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();

        var commandSubscriber = A.Fake<ICommandSubscriber<Guid>>();
        var store = A.Fake<ICommandSubscriberStore>();

        var sut = new StoreCommandSubscriberDecorator(GetServiceProvider(), store, commandSubscriber);
        await sut.RunAsync(new MyCommand());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyCommand).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}

