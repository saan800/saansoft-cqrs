namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class EventRepositoryTests : BaseEventRepositoryTests
{
    private readonly IMongoCollection<Event<Guid>> _messageCollection;
    private readonly EventRepository _repository;

    public EventRepositoryTests()
    {
        _repository = new EventRepository(TestHelpers.GetDatabase(), Logger);
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
