namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreCommandHandlerDecoratorTests : CommandSubscriptionBusDecoratorTestSetup
{
    protected StoreCommandHandlerDecoratorTests()
    {
        _repository = A.Fake<ICommandHandlerRepository<Guid>>();
    }

    private readonly ICommandHandlerRepository<Guid> _repository;
    protected override ICommandSubscriptionBusDecorator<Guid> SutSubscriptionBusDecorator =>
        new StoreCommandHandlerDecorator(_repository, InMemoryCommandBus);

    public class RunAsync : StoreCommandHandlerDecoratorTests
    {
        [Fact]
        public async Task Should_store_single_handler_details()
        {
            await SutSubscriptionBusDecorator.RunAsync(new MyCommand());

            A.CallTo(() => _repository.UpsertHandlerAsync(A<MyCommand>._, typeof(CommandHandler), null, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_zero_handler_details()
        {
            await SutSubscriptionBusDecorator.Invoking(y => y.RunAsync(new NoHandlerCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x => x.Message.Contains("No handler for type"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<NoHandlerCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<NoHandlerCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_not_store_multiple_handlers_details()
        {
            var extraHandler = A.Fake<ICommandHandler<MyCommand>>();
            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => extraHandler);

            await SutSubscriptionBusDecorator.Invoking(y => y.RunAsync(new MyCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x => x.Message.Contains("Only one handler for type"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<MyCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_store_handlers_details_when_next_throws_exception()
        {
            AddCommandHandlerException<NoHandlerCommand>();

            await SutSubscriptionBusDecorator.Invoking(y => y.RunAsync(new NoHandlerCommand()))
                .Should().ThrowAsync<Exception>()
                .Where(x => x.Message.Contains("it went wrong"));

            A.CallTo(() => _repository.UpsertHandlerAsync(A<NoHandlerCommand>._, A<Type>._, null, A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _repository.UpsertHandlerAsync(A<NoHandlerCommand>._, A<Type>._, A<Exception>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}

