namespace SaanSoft.Tests.Cqrs.Decorator.Store;

public class StoreQueryDecoratorTests : QueryBusDecoratorTestSetup
{
    protected StoreQueryDecoratorTests()
    {
        _repository = A.Fake<IQueryRepository>();
    }

    private readonly IQueryRepository _repository;

    protected override IQueryBus SutPublisherDecorator =>
        new StoreQueryDecorator(_repository, InMemoryQueryBus);

    public class FetchAsyncTests : StoreQueryDecoratorTests
    {
        [Theory]
        [AutoFakeData]
        public async Task Should_store_message_details(string message)
        {
            await SutPublisherDecorator.FetchAsync(new MyQuery { Message = message });

            A.CallTo(() => _repository.InsertAsync(A<MyQuery>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Theory]
        [AutoFakeData]
        public async Task IsReplay_should_NOT_store_message_details(string message)
        {
            await SutPublisherDecorator.FetchAsync(new MyQuery { Message = message, IsReplay = true });

            A.CallTo(() => _repository.InsertAsync(A<MyQuery>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

}
