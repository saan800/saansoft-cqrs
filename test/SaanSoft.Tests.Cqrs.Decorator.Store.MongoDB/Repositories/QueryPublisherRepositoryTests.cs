namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class QueryPublisherRepositoryTests : TestSetup
{
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly QueryPublisherRepository _queryPublisherRepository;

    public QueryPublisherRepositoryTests()
    {
        _queryPublisherRepository = new QueryPublisherRepository(Database, IdGenerator);
        _publisherCollection = _queryPublisherRepository.PublisherCollection;
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var query = new MyQuery();
        await _queryPublisherRepository.UpsertPublisherAsync(query, publisherType);

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
        await _queryPublisherRepository.UpsertPublisherAsync(query, publisherType);
        await _queryPublisherRepository.UpsertPublisherAsync(query, publisherType);
        await _queryPublisherRepository.UpsertPublisherAsync(query, publisherType);

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
}
