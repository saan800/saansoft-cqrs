using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventHandlerDecoratorTests : EventSubscriptionBusTestSetup
{
    protected StoreEventHandlerDecoratorTests()
    {
        _repository = A.Fake<IEventHandlerRepository>();
    }

    private readonly IEventHandlerRepository _repository;
    protected override IEventSubscriptionBus SutSubscriptionBus =>
        new StoreEventHandlerDecorator(_repository, InMemoryEventBus);

    public class RunAsync : StoreEventHandlerDecoratorTests
    {
        [Fact]
        public async Task Should_store_single_handler_details()
        {
            await SutSubscriptionBus.RunAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, typeof(EventsHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_zero_handler_details()
        {
            await SutSubscriptionBus.RunAsync(new NoHandlerEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_multiple_handlers_details()
        {
            var handler1 = A.Fake<IBaseEventHandler<MyEvent>>();
            ServiceCollection.AddScoped<IBaseEventHandler<MyEvent>>(_ => handler1);

            await SutSubscriptionBus.RunAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_handlers_details_when_next_throws_exception()
        {
            AddEventHandlerException<NoHandlerEvent>();

            await SutSubscriptionBus.Invoking(y => y.RunAsync(new NoHandlerEvent(Guid.NewGuid())))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("it went wrong"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

