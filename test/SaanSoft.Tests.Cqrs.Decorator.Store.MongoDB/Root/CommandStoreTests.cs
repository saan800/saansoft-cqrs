using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Decorator.Store.Models;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class CommandStoreTests : TestSetup
{
    private readonly IMongoCollection<IMessage<Guid>> _messageCollection;
    private readonly IMongoCollection<MessagePublisherRecord<Guid>> _publisherCollection;
    private readonly IMongoCollection<MessageSubscriberRecord<Guid>> _subscriberCollection;
    private readonly CommandStore _commandStore;

    public CommandStoreTests()
    {
        _commandStore = new CommandStore(Database);
        _messageCollection = _commandStore.MessageCollection;
        _publisherCollection = _commandStore.PublisherCollection;
        _subscriberCollection = _commandStore.SubscriberCollection;
    }

    #region InsertAsync

    [Fact]
    public async Task InsertAsync_can_insert_a_command()
    {
        var message = new MyCommand();
        await _commandStore.InsertAsync(message);

        // check the collection that the command exists
        var record = await _messageCollection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();

        record.Should().NotBeNull();
        record.Id.Should().Be(message.Id);
        record.TypeFullName.Should().Be(typeof(MyCommand).FullName);
        record.GetType().Should().Be<MyCommand>();
    }

    [Fact]
    public async Task InsertAsync_can_insert_and_retrieve_multiple_types_of_commands()
    {
        var message1 = new MyCommand();
        var message2 = new AnotherCommand();
        await _commandStore.InsertAsync(message1);
        await _commandStore.InsertAsync(message2);

        // check the collection that the command exists
        var record1 = await _messageCollection.Find(x => x.Id == message1.Id).FirstOrDefaultAsync();

        record1.Should().NotBeNull();
        record1.Id.Should().Be(message1.Id);
        record1.TypeFullName.Should().Be(typeof(MyCommand).FullName);
        record1.GetType().Should().Be<MyCommand>();
        record1.GetType().Should().NotBe<AnotherCommand>();

        var record2 = await _messageCollection.Find(x => x.Id == message2.Id).FirstOrDefaultAsync();

        record2.Should().NotBeNull();
        record2.Id.Should().Be(message2.Id);
        record2.TypeFullName.Should().Be(typeof(AnotherCommand).FullName);
        record2.GetType().Should().Be<AnotherCommand>();
        record2.GetType().Should().NotBe<MyCommand>();
    }

    #endregion

    #region UpsertPublisherAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_can_insert_record(Type publisherType)
    {
        var command = new MyCommand();
        await _commandStore.UpsertPublisherAsync(command, publisherType);

        // check the collection that the record exists
        var record = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            ).SingleOrDefaultAsync();

        record.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_multiple_times_only_creates_one_record(Type publisherType)
    {
        var command = new MyCommand();
        await _commandStore.UpsertPublisherAsync(command, publisherType);
        await _commandStore.UpsertPublisherAsync(command, publisherType);
        await _commandStore.UpsertPublisherAsync(command, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
                && x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_publisher(Type publisherType1, Type publisherType2)
    {
        var command = new MyCommand();
        await _commandStore.UpsertPublisherAsync(command, publisherType1);
        await _commandStore.UpsertPublisherAsync(command, publisherType2);
        await _commandStore.UpsertPublisherAsync(command, publisherType2);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.MessageTypeName == command.GetType().FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.PublisherTypeName == publisherType1.FullName).Should().Be(1);
        records.Count(x => x.PublisherTypeName == publisherType2.FullName).Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertPublisherAsync_creates_one_record_per_messageName(Type publisherType)
    {
        var command1 = new MyCommand();
        var command2 = new AnotherCommand();
        await _commandStore.UpsertPublisherAsync(command1, publisherType);
        await _commandStore.UpsertPublisherAsync(command1, publisherType);
        await _commandStore.UpsertPublisherAsync(command2, publisherType);

        // check the collection that the record exists
        var records = await _publisherCollection
            .Find(x =>
                x.PublisherTypeName == publisherType.FullName
            )
            .ToListAsync();

        records.Should().NotBeNull();

        records.Count(x => x.MessageTypeName == command1.GetType().FullName).Should().Be(1);
        records.Count(x => x.MessageTypeName == command2.GetType().FullName).Should().Be(1);
    }

    #endregion

    #region UpsertSubscriberAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertSubscriberAsync_can_insert_record(string messageName, string subscriberName)
    {
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName]);

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
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName]);
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName]);

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
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName1, subscriberName2]);
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName1, subscriberName2]);
        await _commandStore.UpsertSubscriberAsync(messageName, [subscriberName2]);

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
        await _commandStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _commandStore.UpsertSubscriberAsync(messageName1, [subscriberName]);
        await _commandStore.UpsertSubscriberAsync(messageName2, [subscriberName]);

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
