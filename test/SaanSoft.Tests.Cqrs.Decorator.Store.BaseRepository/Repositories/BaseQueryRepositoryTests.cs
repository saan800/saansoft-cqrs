namespace SaanSoft.Tests.Cqrs.Decorator.Store.BaseRepository.Repositories;

public abstract class BaseQueryRepositoryTests : TestSetup
{
    protected IQueryRepository SutRepository { get; init; }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_a_query()
    {
        var message = new MyQuery();
        await SutRepository.InsertAsync(message);

        // check the collection that the query exists
        var record = await SutRepository.GetMessageByIdAsync(message.Id); ;

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        record.GetType().Should().Be<MyQuery>();
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_queries()
    {
        var message1 = new MyQuery();
        var message2 = new AnotherQuery();
        await SutRepository.InsertAsync(message1);
        await SutRepository.InsertAsync(message2);

        // check the collection that the query exists
        var record1 = await SutRepository.GetMessageByIdAsync(message1.Id); ;

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        record1.GetType().Should().Be<MyQuery>();
        record1.GetType().Should().NotBe<AnotherQuery>();

        var record2 = await SutRepository.GetMessageByIdAsync(message2.Id); ;

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.TypeFullName.Should().Be(typeof(AnotherQuery).FullName);
        record2.GetType().Should().Be<AnotherQuery>();
        record2.GetType().Should().NotBe<MyQuery>();
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_should_add_handler_to_record(Type handlerType)
    {
        var message = new MyQuery();
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var record = messageHandlers.SingleOrDefault(x => x.TypeFullName == handlerType.FullName);
        record.Should().NotBeNull();
        record.Succeeded.Should().BeTrue();
        record.Exception.Should().BeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_error_should_add_handler_to_record(Type handlerType)
    {
        var message = new MyQuery();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var record = messageHandlers.SingleOrDefault(x => x.TypeFullName == handlerType.FullName);

        record.Should().NotBeNull();
        record.Succeeded.Should().BeFalse();
        record.Exception.TypeFullName.Should().Be("System.Exception");
        record.Exception.Message.Should().Be(exception.Message);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_multiple_times_should_only_add_one_record(Type handlerType)
    {
        var message = new MyQuery();
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.Where(x => x.TypeFullName == handlerType.FullName).ToList();
        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_exception_multiple_times_should_add_multiple_handler_records(Type handlerType)
    {
        var message = new MyQuery();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.Where(x => x.TypeFullName == handlerType.FullName).ToList();
        records.Should().NotBeNull();
        records.Count.Should().Be(3);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_exception_then_without_exception_should_only_have_success_record(Type handlerType)
    {
        var message = new MyQuery();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.FirstOrDefault(x => x.TypeFullName == handlerType.FullName);
        records.Should().NotBeNull();
        records.Succeeded.Should().BeTrue();
    }

    #endregion
}

