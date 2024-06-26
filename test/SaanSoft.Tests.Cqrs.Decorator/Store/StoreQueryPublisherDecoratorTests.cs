namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecoratorTests : QueryBusDecoratorTestSetup
{
    protected StoreQueryPublisherDecoratorTests()
    {
        _repository = A.Fake<IQueryPublisherRepository<Guid>>();
    }

    private readonly IQueryPublisherRepository<Guid> _repository;
    protected override IQueryBusDecorator<Guid> SutPublisherDecorator =>
        new StoreQueryPublisherDecorator(_repository, InMemoryQueryBus);

    public class FetchAsyncTests : StoreQueryPublisherDecoratorTests
    {
        private static readonly Type ExpectedType = typeof(FetchAsyncTests);

        [Fact]
        public async Task FetchAsync_should_store_publisher_details()
        {
            await SutPublisherDecorator.FetchAsync(new MyQuery());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyQuery>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task FetchAsync_multiple_decorators_should_store_publisher_details()
        {
            var wrappedInDecorator = new WrapperQueryBusDecorator(SutPublisherDecorator);

            await wrappedInDecorator.FetchAsync(new MyQuery());

            A.CallTo(() => _repository.UpsertPublisherAsync(A<MyQuery>._, ExpectedType, A<CancellationToken>._)).MustHaveHappened();
        }
    }

    private class WrapperQueryBusDecorator(IQueryBus<Guid> next) : IQueryBus<Guid>
    {
        public Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid>
            => next.FetchAsync(query, cancellationToken);
    }
}
