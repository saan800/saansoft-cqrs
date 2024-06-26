using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task FetchAsync_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();

        var store = A.Fake<IQueryPublisherRepository<Guid>>();
        var sut = new StoreQueryPublisherDecorator(store, InMemoryQueryBus);

        await sut.FetchAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyQuery>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task FetchAsync_multiple_decorators_should_store_publisher_details()
    {
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>, QueryHandler>();

        var store = A.Fake<IQueryPublisherRepository<Guid>>();
        var sut = new StoreQueryPublisherDecorator(store, InMemoryQueryBus);
        var wrappedInDecorator = new WrapperQueryBusDecorator(sut);

        await wrappedInDecorator.FetchAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyQuery>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    private class WrapperQueryBusDecorator(IQueryBus<Guid> next) : IQueryBus<Guid>
    {
        public Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid>
            => next.FetchAsync(query, cancellationToken);
    }
}
