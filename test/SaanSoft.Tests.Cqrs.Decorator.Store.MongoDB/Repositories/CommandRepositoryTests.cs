namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandRepositoryTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _messageCollection;
    private readonly CommandRepository _commandRepository;

    public CommandRepositoryTests()
    {
        _commandRepository = new CommandRepository(Database, IdGenerator);
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
}
