namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class QueryHandlerRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly QueryHandlerRepository _queryHandlerRepository;

    public QueryHandlerRepositoryTests()
    {
        _queryHandlerRepository = new QueryHandlerRepository(Database, IdGenerator);
        _handlerCollection = _queryHandlerRepository.HandlerCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var query = new MyQuery();
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType);

        // check the collection that the record exists
        var record = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == query.GetType().FullName
                && x.HandlerTypeName == handlerType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_multiple_times_only_creates_one_record(Type handlerType)
    {
        var query = new MyQuery();
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType);
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType);
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == query.GetType().FullName
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
        var query = new MyQuery();
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType1);
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType2);
        await _queryHandlerRepository.UpsertHandlerAsync(query, handlerType2);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.MessageTypeName == query.GetType().FullName
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
        var query1 = new MyQuery();
        var query2 = new AnotherQuery();
        await _queryHandlerRepository.UpsertHandlerAsync(query1, handlerType);
        await _queryHandlerRepository.UpsertHandlerAsync(query1, handlerType);
        await _queryHandlerRepository.UpsertHandlerAsync(query2, handlerType);

        // check the collection that the record exists
        var records = await _handlerCollection
            .Find(x =>
                x.HandlerTypeName == handlerType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == query1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == query2.GetType().FullName).Should().Be(1);
    }
}
