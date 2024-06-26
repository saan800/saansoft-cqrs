namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class CommandRepositoryTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _messageCollection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly CommandRepository _commandRepository;

    public CommandRepositoryTests()
    {
        _commandRepository = new CommandRepository(Database);
        _messageCollection = _commandRepository.MessageCollection;
        _publisherCollection = _commandRepository.PublisherCollection;
        _handlerCollection = _commandRepository.HandlerCollection;
    }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_a_command()
    {
        var message = new MyCommand();
        await _commandRepository.InsertAsync(message);

        // check the collection that the command exists
        var record = await _messageCollection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

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
        await _commandRepository.InsertAsync(message1);
        await _commandRepository.InsertAsync(message2);

        // check the collection that the command exists
        var record1 = await _messageCollection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.TypeFullName.Should().Be(typeof(MyCommand).FullName);
        record1.GetType().Should().Be<MyCommand>();
        record1.GetType().Should().NotBe<AnotherCommand>();

        var record2 = await _messageCollection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.TypeFullName.Should().Be(typeof(AnotherCommand).FullName);
        record2.GetType().Should().Be<AnotherCommand>();
        record2.GetType().Should().NotBe<MyCommand>();
    }

    #endregion

    #region UpsertPublisherAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertPublisherAsync(command, publisherType);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
        record.MessageAssembly.Should().Be("SaanSoft.Tests.Cqrs.Common");
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(Type publisherType)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertPublisherAsync(command, publisherType);
        await _commandRepository.UpsertPublisherAsync(command, publisherType);
        await _commandRepository.UpsertPublisherAsync(command, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_publisher(Type publisherType1, Type publisherType2)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertPublisherAsync(command, publisherType1);
        await _commandRepository.UpsertPublisherAsync(command, publisherType2);
        await _commandRepository.UpsertPublisherAsync(command, publisherType2);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.PublisherTypeName == publisherType1.FullName).Should().Be(1);
        records.Count(x => x.PublisherTypeName == publisherType2.FullName).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_messageName(Type publisherType)
    {
        var command1 = new MyCommand();
        var command2 = new AnotherCommand();
        await _commandRepository.UpsertPublisherAsync(command1, publisherType);
        await _commandRepository.UpsertPublisherAsync(command1, publisherType);
        await _commandRepository.UpsertPublisherAsync(command2, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == command1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == command2.GetType().FullName).Should().Be(1);
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertHandlerAsync(command, handlerType);

        // check the collection that the record exists
        var record = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.HandlerTypeName == handlerType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_error(Type handlerType)
    {
        var command = new MyCommand();
        var exception = new Exception("it went wrong");
        await _commandRepository.UpsertHandlerAsync(command, handlerType, exception);

        // check the collection that the record exists
        var record = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.HandlerTypeName == handlerType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
        record.LastMessageId.Should().Be(command.Id);
        record.LastCompletedMessageId.Should().BeNull();
        record.LastFailedMessages.First().MessageId.Should().Be(command.Id);
        record.LastFailedMessages.First().Exception.TypeName.Should().Be("System.Exception");
        record.LastFailedMessages.First().Exception.Message.Should().Be(exception.Message);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_multiple_times_only_creates_one_record(Type handlerType)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertHandlerAsync(command, handlerType);
        await _commandRepository.UpsertHandlerAsync(command, handlerType);
        await _commandRepository.UpsertHandlerAsync(command, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.HandlerTypeName == handlerType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_creates_one_record_per_handler(Type handlerType1, Type handlerType2)
    {
        var command = new MyCommand();
        await _commandRepository.UpsertHandlerAsync(command, handlerType1);
        await _commandRepository.UpsertHandlerAsync(command, handlerType1);
        await _commandRepository.UpsertHandlerAsync(command, handlerType2);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.HandlerTypeName == handlerType1.FullName).Should().Be(1);
        records.Count(x => x.HandlerTypeName == handlerType2.FullName).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_creates_one_record_per_messageName(Type handlerType)
    {
        var command1 = new MyCommand();
        var command2 = new MyCommand();
        await _commandRepository.UpsertHandlerAsync(command1, handlerType);
        await _commandRepository.UpsertHandlerAsync(command1, handlerType);
        await _commandRepository.UpsertHandlerAsync(command2, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.HandlerTypeName == handlerType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == command1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == command2.GetType().FullName).Should().Be(1);
    }

    #endregion
}
