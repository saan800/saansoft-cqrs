using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB;
using SaanSoft.Cqrs.Store.MongoDB.Models;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB.Root;

public class QueryStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly QueryStore _queryStore;

    public QueryStoreTests()
    {
        _queryStore = new QueryStore(Database);
        _collection = _queryStore.MessageCollection;
        _publisherCollection = _queryStore.PublisherCollection;
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

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(string messageName, string publisherName)
    {
        await _queryStore.UpsertPublisherAsync(messageName, publisherName);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.PublisherTypeName == publisherName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(string messageName, string publisherName)
    {
        await _queryStore.UpsertPublisherAsync(messageName, publisherName);
        await _queryStore.UpsertPublisherAsync(messageName, publisherName);
        await _queryStore.UpsertPublisherAsync(messageName, publisherName);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.PublisherTypeName == publisherName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }
}
