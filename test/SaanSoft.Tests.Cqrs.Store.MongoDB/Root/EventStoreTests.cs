using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;
using SaanSoft.Cqrs.Store.MongoDB.Models;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class EventStoreTests : TestSetup
{
    private readonly IMongoCollection<Event> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly EventStore _eventStore;

    public EventStoreTests()
    {
        _eventStore = new EventStore(Database);
        _collection = Database.GetCollection<Event>(_eventStore.MessageCollectionName);
        _publisherCollection = _eventStore.PublisherCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_an_event()
    {
        var entityKey = Guid.NewGuid();
        var message = new MyEvent(entityKey);
        await _eventStore.InsertAsync(message);

        // check the collection that the event exists
        var record = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.Key.Should().Be(entityKey);
        record.Key.Should().Be(message.Key);
        record.TypeFullName.Should().Be(typeof(MyEvent).FullName);
    }

    [Fact]
    public async Task InsertManyAsync_can_insert_events_and_retrieve_all_by_key_ordered_by_message_date()
    {
        var entityKey = Guid.NewGuid();
        var message1 = new MyEvent(entityKey) { MessageOnUtc = DateTime.UtcNow };
        var message2 = new AnotherEvent(entityKey) { MessageOnUtc = DateTime.UtcNow.AddHours(-1) };
        var message3 = new MyEvent(entityKey) { MessageOnUtc = DateTime.UtcNow.AddDays(-1) };
        await _eventStore.InsertManyAsync([message1, message2, message3]);

        // check the collection that the event exists
        var records = await _eventStore.GetAsync(entityKey);

        records.Count.Should().Be(3);

        var messages = new List<Event> { message1, message2, message3 }.OrderBy(x => x.MessageOnUtc).ToList();
        for (var i = 0; i < messages.Count; i++)
        {
            var message = messages[i];
            var record = records[i];

            record.Should().NotBeNull();
            record!.Id.Should().Be(message.Id);
            record.Key.Should().Be(message.Key);
            record.TypeFullName.Should().Be(message.GetType().FullName);
        }
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(string messageName, string publisherName)
    {
        await _eventStore.UpsertPublisherAsync(messageName, publisherName);

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
        await _eventStore.UpsertPublisherAsync(messageName, publisherName);
        await _eventStore.UpsertPublisherAsync(messageName, publisherName);
        await _eventStore.UpsertPublisherAsync(messageName, publisherName);

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
