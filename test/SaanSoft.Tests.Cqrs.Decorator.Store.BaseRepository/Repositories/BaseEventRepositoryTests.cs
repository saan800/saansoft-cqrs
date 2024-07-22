namespace SaanSoft.Tests.Cqrs.Decorator.Store.BaseRepository.Repositories;

public abstract class BaseEventRepositoryTests : TestSetup
{
    protected IEventRepository SutRepository { get; init; }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_an_event()
    {
        var entityKey = Guid.NewGuid();
        var message = new MyEvent(entityKey);
        await SutRepository.InsertAsync(message);

        // check the collection that the event exists
        var record = await SutRepository.GetMessageByIdAsync(message.Id);

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyEvent).FullName);

        // check that have entity record
        var entityRecord = (await SutRepository.GetEntityMessagesAsync(message.Key)).FirstOrDefault();

        entityRecord.Should().NotBeNull();
        entityRecord.Should().BeOfType<MyEvent>();
        entityRecord!.Id.Should().Be(message.Id);
        entityRecord.Key.Should().Be(entityKey);
        entityRecord.Key.Should().Be(message.Key);
        entityRecord.TypeFullName.Should().Be(typeof(MyEvent).FullName);
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_events()
    {
        var message1 = new MyEvent(Guid.NewGuid());
        var message2 = new AnotherEvent(Guid.NewGuid());
        await SutRepository.InsertAsync(message1);
        await SutRepository.InsertAsync(message2);

        // check the collection that the event exists
        var record1 = (IEvent<Guid>?)await SutRepository.GetMessageByIdAsync(message1.Id);

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.Key.Should().Be(message1.Key);
        record1.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        record1.Should().BeOfType<MyEvent>();
        record1.Should().NotBeOfType<AnotherEvent>();

        var record2 = (IEvent<Guid>?)await SutRepository.GetMessageByIdAsync(message2.Id);

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.Key.Should().Be(message2.Key);
        record2.TypeFullName.Should().Be(typeof(AnotherEvent).FullName);
        record2.Should().BeOfType<AnotherEvent>();
        record2.Should().NotBeOfType<MyEvent>();
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_should_add_handler_to_record(Type handlerType)
    {
        var message = new MyEvent(Guid.NewGuid());
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
        var message = new MyEvent(Guid.NewGuid());
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
        var message = new MyEvent(Guid.NewGuid());
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
        var message = new MyEvent(Guid.NewGuid());
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
        var message = new MyEvent(Guid.NewGuid());
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
