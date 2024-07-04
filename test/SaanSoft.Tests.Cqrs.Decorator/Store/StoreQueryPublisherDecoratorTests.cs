namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecoratorTests : QueryBusDecoratorTestSetup
{
    protected override IQueryBusDecorator SutPublisherDecorator =>
        new StoreQueryPublisherDecorator(InMemoryQueryBus);

    public class FetchAsyncTests : StoreQueryPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(FetchAsyncTests);

        [Fact]
        public async Task FetchAsync_should_store_publisher_details()
        {
            var query = new MyQuery();
            await SutPublisherDecorator.FetchAsync(query);

            var publisher = query.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }

        [Fact]
        public async Task FetchAsync_multiple_decorators_should_store_publisher_details()
        {
            var query = new MyQuery();
            var wrappedInDecorator = new WrapperQueryBusDecorator(SutPublisherDecorator);

            await wrappedInDecorator.FetchAsync(query);

            var publisher = query.Metadata.GetValueOrDefaultAs<string>(StoreConstants.PublisherKey);
            publisher.Should().Be(ExpectedType.FullName);
        }
    }

    private class WrapperQueryBusDecorator(IQueryBus next) : IQueryBus
    {
        public Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid>
            => next.FetchAsync(query, cancellationToken);
    }
}
