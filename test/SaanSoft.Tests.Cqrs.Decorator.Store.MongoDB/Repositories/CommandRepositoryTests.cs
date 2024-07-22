namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandRepositoryTests : TestSetup
{
    private readonly IMongoCollection<BaseCommand> _messageCollection;
    private readonly CommandRepository _commandRepository;

    public CommandRepositoryTests()
    {
        _commandRepository = new CommandRepository(Database, Logger);
        _messageCollection = _commandRepository.MessageCollection;
    }

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
        var message3 = new MyCommandWithResponse { Message = "greetings" };
        var message4 = new AnotherCommandWithResponse { Message = "greetings" };
        await _commandRepository.InsertAsync(message1);
        await _commandRepository.InsertAsync(message2);
        await _commandRepository.InsertAsync(message3);
        await _commandRepository.InsertAsync(message4);

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

        var record3 = await _messageCollection.Find(x => x.Id == message3.Id).FirstOrDefaultAsync();
        record3.Should().NotBeNull();
        record3.Id.Should().Be(message3.Id);
        record3.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        record3.GetType().Should().Be<MyCommandWithResponse>();
        record3.GetType().Should().NotBe<MyCommand>();
        record3.GetType().Should().NotBe<AnotherCommandWithResponse>();

        var record4 = await _messageCollection.Find(x => x.Id == message4.Id).FirstOrDefaultAsync();
        record4.Should().NotBeNull();
        record4.Id.Should().Be(message4.Id);
        record4.TypeFullName.Should().Be(typeof(AnotherCommandWithResponse).FullName);
        record4.GetType().Should().Be<AnotherCommandWithResponse>();
        record4.GetType().Should().NotBe<MyCommand>();
        record4.GetType().Should().NotBe<MyCommandWithResponse>();
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync()
    {
        await _commandRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync_can_call_multiple_times()
    {
        await _commandRepository.EnsureCollectionIndexesAsync();
        await _commandRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }
}
