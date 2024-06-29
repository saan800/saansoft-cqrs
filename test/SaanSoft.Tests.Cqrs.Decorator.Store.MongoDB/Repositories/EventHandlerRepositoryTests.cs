namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class EventHandlerRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly EventHandlerRepository _eventHandlerRepository;

    public EventHandlerRepositoryTests()
    {
        _eventHandlerRepository = new EventHandlerRepository(Database, IdGenerator);
        _handlerCollection = _eventHandlerRepository.HandlerCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var evt = new MyEvent(Guid.NewGuid());
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType);

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
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType);
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType);
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType);

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
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType1);
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType2);
        await _eventHandlerRepository.UpsertHandlerAsync(evt, handlerType2);

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
        await _eventHandlerRepository.UpsertHandlerAsync(evt1, handlerType);
        await _eventHandlerRepository.UpsertHandlerAsync(evt1, handlerType);
        await _eventHandlerRepository.UpsertHandlerAsync(evt2, handlerType);

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
}
