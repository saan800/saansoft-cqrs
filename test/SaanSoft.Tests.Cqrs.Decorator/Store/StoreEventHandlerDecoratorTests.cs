namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreEventHandlerDecoratorTests : EventSubscriptionBusDecoratorTestSetup
{
    protected StoreEventHandlerDecoratorTests()
    {
        _repository = A.Fake<IEventRepository>();
    }

    private readonly IEventRepository _repository;

    protected override IEventSubscriptionBus SutSubscriptionBusDecorator =>
        new StoreEventHandlerDecorator(_repository, InMemoryEventBus);

    public class RunAsync : StoreEventHandlerDecoratorTests
    {
        [Fact]
        public async Task Should_store_single_handler_details()
        {
            await SutSubscriptionBusDecorator.RunAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, typeof(EventsHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_zero_handler_details()
        {
            await SutSubscriptionBusDecorator.RunAsync(new NoHandlerEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_multiple_handlers_details()
        {
            var handler1 = A.Fake<IEventHandler<MyEvent>>();
            ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => handler1);

            await SutSubscriptionBusDecorator.RunAsync(new MyEvent(Guid.NewGuid()));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_handlers_details_when_next_throws_exception()
        {
            AddEventHandlerException<NoHandlerEvent>();

            await SutSubscriptionBusDecorator.Invoking(y => y.RunAsync(new NoHandlerEvent(Guid.NewGuid())))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("it went wrong"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<Guid>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

