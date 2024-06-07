using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Bus.Decorator;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store;

namespace SaanSoft.Tests.Cqrs.Bus.Decorator;

public class StoreQueryPublisherDecoratorTests : TestSetup
{
    [Fact]
    public async Task QueryAsync_should_store_publisher_details()
    {
        var queryPublisher = A.Fake<IQueryPublisher<Guid>>();
        var store = A.Fake<IQueryPublisherStore>();

        var sut = new StoreQueryPublisherDecorator(store, queryPublisher);
        await sut.QueryAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyQuery).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_multiple_decorators_should_store_publisher_details()
    {
        var queryPublisher = A.Fake<IQueryPublisher<Guid>>();
        var store = A.Fake<IQueryPublisherStore>();

        var sut = new StoreQueryPublisherDecorator(store, queryPublisher);
        var wrappedInDecorator = new WrapperQueryPublisher(sut);

        await wrappedInDecorator.QueryAsync(new MyQuery());

        A.CallTo(() => store.UpsertPublisherAsync(typeof(MyQuery).FullName!, this.GetType().FullName!, A<CancellationToken>._)).MustHaveHappened();
    }

    private class WrapperQueryPublisher(IQueryPublisher<Guid> next) : IQueryPublisher<Guid>
    {
        public Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
            CancellationToken cancellationToken = default) where TQuery :
            IQuery<TQuery, TResponse>
            where TResponse : IQueryResponse
            => next.QueryAsync(query, cancellationToken);
    }
}
