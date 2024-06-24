using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Decorator.Store.Models;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class EventRepositoryTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly EventRepository _eventRepository;

    public EventRepositoryTests()
    {
        _eventRepository = new EventRepository(Database);
        _collection = _eventRepository.MessageCollection;
        _publisherCollection = _eventRepository.PublisherCollection;
        _handlerCollection = _eventRepository.HandlerCollection;
    }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_an_event()
    {
        var entityKey = Guid.NewGuid();
        var message = new MyEvent(entityKey);
        await _eventRepository.InsertAsync(message);

        // check the collection that the event exists
        var record = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyEvent).FullName);

        // check that have entity record
        var entityRecord = (await _eventRepository.GetEntityMessagesAsync(message.Key)).FirstOrDefault();

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
        await _eventRepository.InsertAsync(message1);
        await _eventRepository.InsertAsync(message2);

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
        await _eventRepository.InsertManyAsync([message1, message2, message3]);

        // check the collection that the event exists
        var records = await _eventRepository.GetEntityMessagesAsync(entityKey);

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
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertPublisherAsync(evt, publisherType);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(Type publisherType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertPublisherAsync(evt, publisherType);
        await _eventRepository.UpsertPublisherAsync(evt, publisherType);
        await _eventRepository.UpsertPublisherAsync(evt, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
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
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertPublisherAsync(evt, publisherType1);
        await _eventRepository.UpsertPublisherAsync(evt, publisherType2);
        await _eventRepository.UpsertPublisherAsync(evt, publisherType2);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
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
        var event1 = new MyEvent(Guid.NewGuid());
        var event2 = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertPublisherAsync(event1, publisherType);
        await _eventRepository.UpsertPublisherAsync(event1, publisherType);
        await _eventRepository.UpsertPublisherAsync(event2, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == event1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == event2.GetType().FullName).Should().Be(1);
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertHandlerAsync(evt, handlerType);

        // check the collection that the record exists
        var record = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
                && x.HandlerTypeName == handlerType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_multiple_times_only_creates_one_record(Type handlerType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertHandlerAsync(evt, handlerType);
        await _eventRepository.UpsertHandlerAsync(evt, handlerType);
        await _eventRepository.UpsertHandlerAsync(evt, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
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
        var evt = new MyEvent(Guid.NewGuid());
        await _eventRepository.UpsertHandlerAsync(evt, handlerType1);
        await _eventRepository.UpsertHandlerAsync(evt, handlerType2);
        await _eventRepository.UpsertHandlerAsync(evt, handlerType2);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == evt.GetType().FullName
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
        var evt1 = new MyEvent(Guid.NewGuid());
        var evt2 = new AnotherEvent(Guid.NewGuid());
        await _eventRepository.UpsertHandlerAsync(evt1, handlerType);
        await _eventRepository.UpsertHandlerAsync(evt1, handlerType);
        await _eventRepository.UpsertHandlerAsync(evt2, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.HandlerTypeName == handlerType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == evt1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == evt2.GetType().FullName).Should().Be(1);
    }

    #endregion
}
