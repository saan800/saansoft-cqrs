using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;
using SaanSoft.Cqrs.Decorator.Store.MongoDB.Models;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class QueryStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _collection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageSubscriberRecord<Guid>> _subscriberCollection;
    private readonly QueryStore _queryStore;

    public QueryStoreTests()
    {
        _queryStore = new QueryStore(Database);
        _collection = _queryStore.MessageCollection;
        _publisherCollection = _queryStore.PublisherCollection;
        _subscriberCollection = _queryStore.SubscriberCollection;
    }

    #region InsertAsync

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
        record.GetType().Should().Be<MyQuery>();
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_queries()
    {
        var message1 = new MyQuery();
        var message2 = new AnotherQuery();
        await _queryStore.InsertAsync(message1);
        await _queryStore.InsertAsync(message2);

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

    #endregion

    #region UpsertSubscriberAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_can_insert_record(string messageName, string subscriberName)
    {
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName]);

        // check the collection that the record exists
        var record = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.SubscriberTypeName == subscriberName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_multiple_times_only_creates_one_record(string messageName, string subscriberName)
    {
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
                && x.SubscriberTypeName == subscriberName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_creates_one_record_per_subscriber(string messageName, string subscriberName1, string subscriberName2)
    {
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName1, subscriberName2]);
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName2]);
        await _queryStore.UpsertSubscriberAsync(messageName, [subscriberName2]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.MessageTypeName == messageName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.SubscriberTypeName == subscriberName1).Should().Be(1);
        records.Count(x => x.SubscriberTypeName == subscriberName2).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_creates_one_record_per_messageName(string messageName1, string messageName2, string subscriberName)
    {
        await _queryStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _queryStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _queryStore.UpsertSubscriberAsync(messageName2, [subscriberName]);

        // check the collection that the record exists
        var records = await _subscriberCollection
            .Find(x =>
                x.SubscriberTypeName == subscriberName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == messageName1).Should().Be(1);
        records.Count(x => x.MessageTypeName == messageName2).Should().Be(1);
    }

    #endregion
}
