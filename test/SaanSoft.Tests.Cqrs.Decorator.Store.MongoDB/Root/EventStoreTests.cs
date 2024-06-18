using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;
using SaanSoft.Cqrs.Decorator.Store.MongoDB.Models;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class EventStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageSubscriberRecord<Guid>> _subscriberCollection;
    private readonly EventStore _eventStore;

    public EventStoreTests()
    {
        _eventStore = new EventStore(Database);
        _collection = _eventStore.MessageCollection;
        _publisherCollection = _eventStore.PublisherCollection;
        _subscriberCollection = _eventStore.SubscriberCollection;
    }

    #region InsertAsync

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
        record.TypeFullName.Should().Be(typeof(MyEvent).FullName);

        // check that have entity record
        var entityRecord = (await _eventStore.GetEntityMessagesAsync(message.Key)).FirstOrDefault();

        entityRecord.Should().NotBeNull();
        entityRecord!.Id.Should().Be(message.Id);
        entityRecord.Key.Should().Be(entityKey);
        entityRecord.Key.Should().Be(message.Key);
        entityRecord.TypeFullName.Should().Be(typeof(MyEvent).FullName);
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_events()
    {
        var message1 = new MyEvent(Guid.NewGuid());
        var message2 = new AnotherEvent(Guid.NewGuid());
        await _eventStore.InsertAsync(message1);
        await _eventStore.InsertAsync(message2);

        // check the collection that the event exists
        var record1 = (IEvent<Guid, Guid>)await _collection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.Key.Should().Be(message1.Key);
        record1.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        record1.GetType().Should().Be<MyEvent>();
        record1.GetType().Should().NotBe<AnotherEvent>();

        var record2 = (IEvent<Guid, Guid>)await _collection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.Key.Should().Be(message2.Key);
        record2.TypeFullName.Should().Be(typeof(AnotherEvent).FullName);
        record2.GetType().Should().Be<AnotherEvent>();
        record2.GetType().Should().NotBe<MyEvent>();
    }

    #endregion

    #region InsertManyAsync

    [Fact]
    public async Task InsertManyAsync_can_insert_events_and_retrieve_all_by_key_ordered_by_message_date()
    {
        var entityKey = Guid.NewGuid();
        var message1 = new MyEvent(entityKey) { MessageOnUtc = DateTime.UtcNow };
        var message2 = new AnotherEvent(entityKey) { MessageOnUtc = DateTime.UtcNow.AddHours(-1) };
        var message3 = new MyEvent(entityKey) { MessageOnUtc = DateTime.UtcNow.AddDays(-1) };
        await _eventStore.InsertManyAsync([message1, message2, message3]);

        // check the collection that the event exists
        var records = await _eventStore.GetEntityMessagesAsync(entityKey);

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

    #endregion

    #region UpsertPublisherAsync

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

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_publisher(string messageName, string publisherName1, string publisherName2)
    {
        await _eventStore.UpsertPublisherAsync(messageName, publisherName1);
        await _eventStore.UpsertPublisherAsync(messageName, publisherName2);
        await _eventStore.UpsertPublisherAsync(messageName, publisherName2);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == messageName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.PublisherTypeName == publisherName1).Should().Be(1);
        records.Count(x => x.PublisherTypeName == publisherName2).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_messageName(string messageName1, string messageName2, string publisherName)
    {
        await _eventStore.UpsertPublisherAsync(messageName1, publisherName);
        await _eventStore.UpsertPublisherAsync(messageName1, publisherName);
        await _eventStore.UpsertPublisherAsync(messageName2, publisherName);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.PublisherTypeName == publisherName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == messageName1).Should().Be(1);
        records.Count(x => x.MessageTypeName == messageName2).Should().Be(1);
    }

    #endregion

    #region UpsertSubscriberAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_can_insert_record(string messageName, string subscriberName)
    {
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName]);

        // check the collection that the record exists
        var record = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.SubscriberTypeName == subscriberName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_multiple_times_only_creates_one_record(string messageName, string subscriberName)
    {
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.SubscriberTypeName == subscriberName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_creates_one_record_per_subscriber(string messageName, string subscriberName1, string subscriberName2)
    {
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName1, subscriberName2]);
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName2]);
        await _eventStore.UpsertSubscriberAsync(messageName, [subscriberName2]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.SubscriberTypeName == subscriberName1).Should().Be(1);
        records.Count(x => x.SubscriberTypeName == subscriberName2).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_creates_one_record_per_messageName(string messageName1, string messageName2, string subscriberName)
    {
        await _eventStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _eventStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _eventStore.UpsertSubscriberAsync(messageName2, [subscriberName]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.SubscriberTypeName == subscriberName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == messageName1).Should().Be(1);
        records.Count(x => x.MessageTypeName == messageName2).Should().Be(1);
    }

    #endregion
}
