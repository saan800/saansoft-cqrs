using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;
using QueryResponse = SaanSoft.Tests.Cqrs.Common.TestModels.QueryResponse;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQuerySubscriberDecoratorTests : TestSetup
{
    [Fact]
    public async Task RunAsync_should_store_single_subscriber_details()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>, QueryHandler>();

        var querySubscriber = A.Fake<IQuerySubscriber<Guid>>();
        var store = A.Fake<IQuerySubscriberStore>();

        var sut = new StoreQuerySubscriberDecorator(GetServiceProvider(), store, querySubscriber);
        await sut.RunAsync(new MyQuery());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyQuery).FullName!, A<IEnumerable<string>>.That.Contains(typeof(QueryHandler).FullName!), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RunAsync_should_not_store_zero_subscriber_details()
    {
        var querySubscriber = A.Fake<IQuerySubscriber<Guid>>();
        var store = A.Fake<IQuerySubscriberStore>();

        var sut = new StoreQuerySubscriberDecorator(GetServiceProvider(), store, querySubscriber);
        await sut.RunAsync(new MyQuery());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyQuery).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task RunAsync_should_not_store_multiple_subscribers_details()
    {
        var handler1 = A.Fake<IQueryHandler<MyQuery, QueryResponse>>();
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler1);
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>, QueryHandler>();

        var querySubscriber = A.Fake<IQuerySubscriber<Guid>>();
        var store = A.Fake<IQuerySubscriberStore>();

        var sut = new StoreQuerySubscriberDecorator(GetServiceProvider(), store, querySubscriber);
        await sut.RunAsync(new MyQuery());

        A.CallTo(() => store.UpsertSubscriberAsync(typeof(MyQuery).FullName!, A<IEnumerable<string>>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}
