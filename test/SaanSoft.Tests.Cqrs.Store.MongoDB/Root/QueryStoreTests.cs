using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class QueryStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly QueryStore _queryStore;

    public QueryStoreTests()
    {
        _queryStore = new QueryStore(Database);
        _collection = _queryStore.MessageCollection;
    }

    [Fact]
    public async Task InsertAsync_can_insert_a_query()
    {
        var message = new MyQuery();
        await _queryStore.InsertAsync(message);

        // check the collection that the query exists
        var record = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyQuery).FullName);
    }
}
