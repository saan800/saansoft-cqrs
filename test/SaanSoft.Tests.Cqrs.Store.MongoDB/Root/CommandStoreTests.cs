using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class CommandStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly CommandStore _commandStore;

    public CommandStoreTests()
    {
        _commandStore = new CommandStore(Database);
        _collection = _commandStore.MessageCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_a_command()
    {
        var message = new MyCommand();
        await _commandStore.InsertAsync(message);

        // check the collection that the command exists
        var record = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }
}
