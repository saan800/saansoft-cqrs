namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class QueryRepositoryTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageHandlerRecord<Guid>> _handlerCollection;
    private readonly QueryRepository _queryRepository;

    public QueryRepositoryTests()
    {
        _queryRepository = new QueryRepository(Database);
        _collection = _queryRepository.MessageCollection;
        _publisherCollection = _queryRepository.PublisherCollection;
        _handlerCollection = _queryRepository.HandlerCollection;
    }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_a_query()
    {
        var message = new MyQuery();
        await _queryRepository.InsertAsync(message);

        // check the collection that the query exists
        var record = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        record.GetType().Should().Be<MyQuery>();
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_queries()
    {
        var message1 = new MyQuery();
        var message2 = new AnotherQuery();
        await _queryRepository.InsertAsync(message1);
        await _queryRepository.InsertAsync(message2);

        // check the collection that the query exists
        var record1 = await _collection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        record1.GetType().Should().Be<MyQuery>();
        record1.GetType().Should().NotBe<AnotherQuery>();

        var record2 = await _collection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.TypeFullName.Should().Be(typeof(AnotherQuery).FullName);
        record2.GetType().Should().Be<AnotherQuery>();
        record2.GetType().Should().NotBe<MyQuery>();
    }

    #endregion

    #region UpsertPublisherAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var query = new MyQuery();
        await _queryRepository.UpsertPublisherAsync(query, publisherType);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == query.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(Type publisherType)
    {
        var query = new MyQuery();
        await _queryRepository.UpsertPublisherAsync(query, publisherType);
        await _queryRepository.UpsertPublisherAsync(query, publisherType);
        await _queryRepository.UpsertPublisherAsync(query, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == query.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    #endregion

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_can_insert_record(Type handlerType)
    {
        var query = new MyQuery();
        await _queryRepository.UpsertHandlerAsync(query, handlerType);

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
        await _queryRepository.UpsertHandlerAsync(query, handlerType);
        await _queryRepository.UpsertHandlerAsync(query, handlerType);
        await _queryRepository.UpsertHandlerAsync(query, handlerType);

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
        await _queryRepository.UpsertHandlerAsync(query, handlerType1);
        await _queryRepository.UpsertHandlerAsync(query, handlerType2);
        await _queryRepository.UpsertHandlerAsync(query, handlerType2);

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
        await _queryRepository.UpsertHandlerAsync(query1, handlerType);
        await _queryRepository.UpsertHandlerAsync(query1, handlerType);
        await _queryRepository.UpsertHandlerAsync(query2, handlerType);

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

    #endregion
}
