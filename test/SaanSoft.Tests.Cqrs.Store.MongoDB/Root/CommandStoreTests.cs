using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;
using SaanSoft.Cqrs.Store.MongoDB.Models;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class CommandStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _messageCollection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly CommandStore _commandStore;

    public CommandStoreTests()
    {
        _commandStore = new CommandStore(Database);
        _messageCollection = _commandStore.MessageCollection;
        _publisherCollection = _commandStore.PublisherCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_a_command()
    {
        var message = new MyCommand();
        await _commandStore.InsertAsync(message);

        // check the collection that the command exists
        var record = await _messageCollection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(string messageName, string publisherName)
    {
        await _commandStore.UpsertPublisherAsync(messageName, publisherName);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.PublisherTypeName == publisherName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(string messageName, string publisherName)
    {
        await _commandStore.UpsertPublisherAsync(messageName, publisherName);
        await _commandStore.UpsertPublisherAsync(messageName, publisherName);
        await _commandStore.UpsertPublisherAsync(messageName, publisherName);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.PublisherTypeName == publisherName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }
}
