using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;
using SaanSoft.Tests.Cqrs.Common.TestSubscribers;
using QueryResponse = SaanSoft.Tests.Cqrs.Common.TestModels.QueryResponse;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQuerySubscriberDecoratorTests : TestSetup
{
    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>, QueryHandler>();

        var querySubscriber = new TestQuerySubscriber(GetServiceProvider());
        var store = A.Fake<IQuerySubscriberStore<Guid>>();

        var sut = new StoreQuerySubscriberDecorator(store, querySubscriber);
        await sut.RunAsync(new MyQuery());

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, typeof(QueryHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var querySubscriber = new TestQuerySubscriber(GetServiceProvider());
        var store = A.Fake<IQuerySubscriberStore<Guid>>();

        var sut = new StoreQuerySubscriberDecorator(store, querySubscriber);
        await sut.Invoking(y => y.RunAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("No service for typ"));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<IQueryHandler<MyQuery, QueryResponse>>();
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler1);
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>, QueryHandler>();

        var querySubscriber = new TestQuerySubscriber(GetServiceProvider());
        var store = A.Fake<IQuerySubscriberStore<Guid>>();

        var sut = new StoreQuerySubscriberDecorator(store, querySubscriber);
        await sut.Invoking(y => y.RunAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x => x.Message.StartsWith("Only one service for type"));

        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => store.UpsertSubscriberAsync(A<MyQuery>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
    }
}
