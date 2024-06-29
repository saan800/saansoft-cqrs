namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class EventPublisherRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly EventPublisherRepository _eventPublisherRepository;

    public EventPublisherRepositoryTests()
    {
        _eventPublisherRepository = new EventPublisherRepository(Database, IdGenerator);
        _publisherCollection = _eventPublisherRepository.PublisherCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType);

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
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType);
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType);
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType);

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
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType1);
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType2);
        await _eventPublisherRepository.UpsertPublisherAsync(evt, publisherType2);

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
        await _eventPublisherRepository.UpsertPublisherAsync(event1, publisherType);
        await _eventPublisherRepository.UpsertPublisherAsync(event1, publisherType);
        await _eventPublisherRepository.UpsertPublisherAsync(event2, publisherType);

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
}
