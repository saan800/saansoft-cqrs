namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class QueryRepositoryTests : TestSetup
{
    private readonly IMongoCollection<Query<Guid>> _messageCollection;
    private readonly QueryRepository _queryRepository;

    public QueryRepositoryTests()
    {
        _queryRepository = new QueryRepository(Database, IdGenerator, Logger);
        _messageCollection = _queryRepository.MessageCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_a_query()
    {
        var message = new MyQuery();
        await _queryRepository.InsertAsync(message);

        // check the collection that the query exists
        var record = await _messageCollection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.Metadata.TypeFullName.Should().Be(typeof(MyQuery).FullName);
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
        var record1 = await _messageCollection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.Metadata.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        record1.GetType().Should().Be<MyQuery>();
        record1.GetType().Should().NotBe<AnotherQuery>();

        var record2 = await _messageCollection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.Metadata.TypeFullName.Should().Be(typeof(AnotherQuery).FullName);
        record2.GetType().Should().Be<AnotherQuery>();
        record2.GetType().Should().NotBe<MyQuery>();
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync()
    {
        await _queryRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync_can_call_multiple_times()
    {
        await _queryRepository.EnsureCollectionIndexesAsync();
        await _queryRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }
}
