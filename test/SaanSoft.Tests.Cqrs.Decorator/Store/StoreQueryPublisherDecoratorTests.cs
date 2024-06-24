using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task QueryAsync_should_store_publisher_details()
    {
        var queryPublisher = A.Fake<IQueryBus<Guid>>();
        var store = A.Fake<IQueryPublisherRepository<Guid>>();

        var sut = new StoreQueryPublisherDecorator(store, queryPublisher);
        await sut.QueryAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyQuery>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_multiple_decorators_should_store_publisher_details()
    {
        var queryPublisher = A.Fake<IQueryBus<Guid>>();
        var store = A.Fake<IQueryPublisherRepository<Guid>>();

        var sut = new StoreQueryPublisherDecorator(store, queryPublisher);
        var wrappedInDecorator = new WrapperQueryBusDecorator(sut);

        await wrappedInDecorator.QueryAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(A<MyQuery>._, this.GetType(), A<CancellationToken>._)).MustHaveHappened();
    }

    private class WrapperQueryBusDecorator(IQueryBus<Guid> next) : IQueryBus<Guid>
    {
        public Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid>
            => next.QueryAsync(query, cancellationToken);
    }
}
