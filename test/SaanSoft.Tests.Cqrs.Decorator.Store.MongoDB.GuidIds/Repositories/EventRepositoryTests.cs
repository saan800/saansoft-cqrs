using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class EventRepositoryTests : TestSetup
{
    private readonly IMongoCollection<BaseEvent<Guid, Guid>> _messageCollection;
    private readonly EventRepository _eventRepository;

    public EventRepositoryTests()
    {
        _eventRepository = new EventRepository(Database, IdGenerator, Logger);
        _messageCollection = _eventRepository.MessageCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_an_event()
    {
        var entityKey = Guid.NewGuid();
        var message = new MyEvent(entityKey);
        await _eventRepository.InsertAsync(message);

        // check the collection that the event exists
        var record = await _messageCollection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);

        // check that have entity record
        var entityRecord = (await _eventRepository.GetEntityMessagesAsync(message.Key)).FirstOrDefault();

        entityRecord.Should().NotBeNull();
        entityRecord.Should().BeOfType<MyEvent>();
        entityRecord!.Id.Should().Be(message.Id);
        entityRecord.Key.Should().Be(entityKey);
        entityRecord.Key.Should().Be(message.Key);
        entityRecord.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_events()
    {
        var message1 = new MyEvent(Guid.NewGuid());
        var message2 = new AnotherEvent(Guid.NewGuid());
        await _eventRepository.InsertAsync(message1);
        await _eventRepository.InsertAsync(message2);

        // check the collection that the event exists
        var record1 = (IBaseEvent<Guid, Guid>)await _messageCollection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.Key.Should().Be(message1.Key);
        record1.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        record1.Should().BeOfType<MyEvent>();
        record1.Should().NotBeOfType<AnotherEvent>();

        var record2 = (IBaseEvent<Guid, Guid>)await _messageCollection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.Key.Should().Be(message2.Key);
        record2.Metadata.TypeFullName.Should().Be(typeof(AnotherEvent).FullName);
        record2.Should().BeOfType<AnotherEvent>();
        record2.Should().NotBeOfType<MyEvent>();
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync()
    {
        await _eventRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync_can_call_multiple_times()
    {
        await _eventRepository.EnsureCollectionIndexesAsync();
        await _eventRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }
}
