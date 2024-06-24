using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventMongoDbStore<TMessageId, TEntityKey> : IEventStore<TMessageId, TEntityKey>, IMongoDbStore<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    IMongoCollection<IEvent<TMessageId, TEntityKey>> EventMessageCollection { get; }
}

public class EventStore(IMongoDatabase database)
    : EventStore<Guid>(database)
{
}

public class EventStore<TEntityKey>(IMongoDatabase database)
    : EventStore<Guid, TEntityKey>(database)
    where TEntityKey : struct
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

/// <summary>
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
/// <param name="database"></param>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public abstract class EventStore<TMessageId, TEntityKey>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, IEvent<TMessageId>>(database),
    IEventMongoDbStore<TMessageId, TEntityKey>,
    IEventPublisherStore<TMessageId>,
    IEventSubscriberStore<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    public override string MessageCollectionName => "EventMessages";
    public override string PublisherCollectionName => "EventPublishers";
    public override string SubscriberCollectionName => "EventSubscribers";

    public IMongoCollection<IEvent<TMessageId, TEntityKey>> EventMessageCollection => Database.GetCollection<IEvent<TMessageId, TEntityKey>>(MessageCollectionName);

    public async Task<List<IEvent<TMessageId, TEntityKey>>> GetEntityMessagesAsync(TEntityKey key,
        CancellationToken cancellationToken = default)
        => (await EventMessageCollection
            .Find(x => x.Key.Equals(key))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.MessageOnUtc)
            .ToList();

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureMessageCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = EventMessageCollection.Indexes;

        var keyIndex = Builders<IEvent<TMessageId, TEntityKey>>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.TriggeredByUser)
            .Ascending(x => x.MessageOnUtc);

        await indexes.CreateOneAsync(
            new CreateIndexModel<IEvent<TMessageId, TEntityKey>>(keyIndex, new CreateIndexOptions { Unique = false }),
            null,
            cancellationToken
        );
    }
}
