namespace SaanSoft.Tests.Cqrs.Decorator.Store.BaseRepository.Repositories;

public abstract class BaseCommandRepositoryTests : TestSetup
{
    protected ICommandRepository SutRepository { get; init; }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_a_command()
    {
        var message = new MyCommand();
        await SutRepository.InsertAsync(message);

        // check the collection that the command exists
        var record = await SutRepository.GetMessageByIdAsync(message.Id);

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyCommand).FullName);
        record.GetType().Should().Be<MyCommand>();
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_commands()
    {
        var message1 = new MyCommand();
        var message2 = new AnotherCommand();
        var message3 = new MyCommandWithResponse { Message = "greetings" };
        var message4 = new AnotherCommandWithResponse { Message = "greetings" };
        await SutRepository.InsertAsync(message1);
        await SutRepository.InsertAsync(message2);
        await SutRepository.InsertAsync(message3);
        await SutRepository.InsertAsync(message4);

        // check the collection that the command exists
        var record1 = await SutRepository.GetMessageByIdAsync(message1.Id);
        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.TypeFullName.Should().Be(typeof(MyCommand).FullName);
        record1.GetType().Should().Be<MyCommand>();
        record1.GetType().Should().NotBe<AnotherCommand>();

        var record2 = await SutRepository.GetMessageByIdAsync(message2.Id);
        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.TypeFullName.Should().Be(typeof(AnotherCommand).FullName);
        record2.GetType().Should().Be<AnotherCommand>();
        record2.GetType().Should().NotBe<MyCommand>();

        var record3 = await SutRepository.GetMessageByIdAsync(message3.Id);
        record3.Should().NotBeNull();
        record3.Id.Should().Be(message3.Id);
        record3.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        record3.GetType().Should().Be<MyCommandWithResponse>();
        record3.GetType().Should().NotBe<MyCommand>();
        record3.GetType().Should().NotBe<AnotherCommandWithResponse>();

        var record4 = await SutRepository.GetMessageByIdAsync(message4.Id);
        record4.Should().NotBeNull();
        record4.Id.Should().Be(message4.Id);
        record4.TypeFullName.Should().Be(typeof(AnotherCommandWithResponse).FullName);
        record4.GetType().Should().Be<AnotherCommandWithResponse>();
        record4.GetType().Should().NotBe<MyCommand>();
        record4.GetType().Should().NotBe<MyCommandWithResponse>();
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_should_add_handler_to_record(Type handlerType)
    {
        var message = new MyCommand();
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
        var message = new MyCommand();
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
        var message = new MyCommand();
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
        var message = new MyCommand();
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
        var message = new MyCommand();
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
