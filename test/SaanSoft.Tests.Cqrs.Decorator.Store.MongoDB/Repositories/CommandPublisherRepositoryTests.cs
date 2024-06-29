namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandPublisherRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly CommandPublisherRepository _commandPublisherRepository;

    public CommandPublisherRepositoryTests()
    {
        _commandPublisherRepository = new CommandPublisherRepository(Database, IdGenerator);
        _publisherCollection = _commandPublisherRepository.PublisherCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var command = new MyCommand();
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType);

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
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType);
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType);
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType);

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
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType1);
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType2);
        await _commandPublisherRepository.UpsertPublisherAsync(command, publisherType2);

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
        await _commandPublisherRepository.UpsertPublisherAsync(command1, publisherType);
        await _commandPublisherRepository.UpsertPublisherAsync(command1, publisherType);
        await _commandPublisherRepository.UpsertPublisherAsync(command2, publisherType);

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
}
