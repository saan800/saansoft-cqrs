namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandRepositoryTests : BaseCommandRepositoryTests
{
    private readonly CommandRepository _commandRepository;
    private readonly IMongoCollection<BaseCommand> _messageCollection;

    public CommandRepositoryTests()
    {
        _commandRepository = new CommandRepository(TestHelpers.GetDatabase(), Logger);
        SutRepository = _commandRepository;
        _messageCollection = _commandRepository.MessageCollection;
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync()
    {
        await _commandRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }

    [Fact]
    public async Task EnsureCollectionIndexesAsync_can_call_multiple_times()
    {
        await _commandRepository.EnsureCollectionIndexesAsync();
        await _commandRepository.EnsureCollectionIndexesAsync();

        var indexDocuments = await (await _messageCollection.Indexes.ListAsync()).ToListAsync();
        indexDocuments.Count.Should().Be(2); // one for Id, and one for our index
    }
}
