using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryHandlerDecoratorTests : QuerySubscriptionBusTestSetup
{
    protected StoreQueryHandlerDecoratorTests()
    {
        _repository = A.Fake<IQueryHandlerRepository>();
    }

    private readonly IQueryHandlerRepository _repository;
    protected override IQuerySubscriptionBus SutSubscriptionBus =>
        new StoreQueryHandlerDecorator(_repository, InMemoryQueryBus);

    public class RunAsync : StoreQueryHandlerDecoratorTests
    {
        [Fact]
        public async Task Should_store_single_handler_details()
        {
            await SutSubscriptionBus.RunAsync(new MyQuery());

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, typeof(QueryHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_zero_handler_details()
        {
            await SutSubscriptionBus.Invoking(y => y.RunAsync(new NoHandlerQuery()))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("No handler for type"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_multiple_handlers_details()
        {
            var handler1 = A.Fake<IBaseQueryHandler<MyQuery, MyQueryResponse>>();
            ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>>(_ => handler1);

            await SutSubscriptionBus.Invoking(y => y.RunAsync(new MyQuery()))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("Only one handler for type"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_handlers_details_when_next_throws_exception()
        {
            AddQueryHandlerException<NoHandlerQuery, string>();

            await SutSubscriptionBus.Invoking(y => y.RunAsync(new NoHandlerQuery()))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("it went wrong"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
