namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class QueryRepositoryTests : BaseQueryRepositoryTests
{
    private readonly QueryRepository _repository;
    private readonly IMongoCollection<Query> _messageCollection;

    public QueryRepositoryTests()
    {
        _repository = new QueryRepository(TestHelpers.GetDatabase(), Logger);
        SutRepository = _repository;
        _messageCollection = _repository.MessageCollection;
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync()
    {
        await _repository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync_can_call_multiple_times()
    {
        await _repository.EnsureCollectionIndexesAsync();
        await _repository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }
}
