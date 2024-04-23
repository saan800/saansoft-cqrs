using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class CommandStoreTests : TestSetup
{
    private readonly IMongoCollection<Command> _collection;
    private readonly CommandStore _commandStore;

    public CommandStoreTests()
    {
        _commandStore = new CommandStore(Database);
        _collection = Database.GetCollection<Command>(_commandStore.MessageCollectionName);
    }

    [Fact]
    public async Task InsertAsync_can_insert_a_message()
    {
        var message = new MyCommand { Id = Guid.NewGuid() };
        await _commandStore.InsertAsync(message);

        // check the collection that it exists
        var record = (MyCommand)await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }
}
