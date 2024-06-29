namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandHandlerRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly CommandHandlerRepository _commandHandlerRepository;

    public CommandHandlerRepositoryTests()
    {
        _commandHandlerRepository = new CommandHandlerRepository(Database, IdGenerator);
        _handlerCollection = _commandHandlerRepository.HandlerCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var command = new MyCommand();
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType);

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
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType, exception);

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
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType);
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType);
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType);

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
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType1);
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType1);
        await _commandHandlerRepository.UpsertHandlerAsync(command, handlerType2);

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
        await _commandHandlerRepository.UpsertHandlerAsync(command1, handlerType);
        await _commandHandlerRepository.UpsertHandlerAsync(command1, handlerType);
        await _commandHandlerRepository.UpsertHandlerAsync(command2, handlerType);

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
}
