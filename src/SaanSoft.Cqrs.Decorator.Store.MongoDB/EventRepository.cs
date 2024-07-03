namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventMongoDbRepository<TMessageId, TEntityKey> :
    IEventRepository<TMessageId, TEntityKey>,
    IMongoDbRepository
    where TMessageId : struct
    where TEntityKey : struct
{
    IMongoCollection<Event<TMessageId, TEntityKey>> MessageCollection { get; }
}

public class EventRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid>(database, idGenerator, insertOneOptions)
{
}

public class EventRepository<TEntityKey>(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid, TEntityKey>(database, idGenerator, insertOneOptions)
    where TEntityKey : struct
{
}

/// <summary>
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
/// <param name="database"></param>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public abstract class EventRepository<TMessageId, TEntityKey>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, InsertOneOptions? insertOneOptions = null) :
    BaseMessageRepository<TMessageId, IEvent<TMessageId>>(database, idGenerator, insertOneOptions),
    IEventMongoDbRepository<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public override string CollectionName => "EventMessages";

    public IMongoCollection<Event<TMessageId, TEntityKey>> MessageCollection
        => Database.GetCollection<Event<TMessageId, TEntityKey>>(CollectionName);

    public async Task<List<IEvent<TMessageId, TEntityKey>>> GetEntityMessagesAsync(TEntityKey key,
        CancellationToken cancellationToken = default)
        => (await MessageCollection
            .Find(x => x.Key.Equals(key))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.MessageOnUtc)
            .Select(x => (IEvent<TMessageId, TEntityKey>)x)
            .ToList();

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = MessageCollection.Indexes;

        var keyIndex = Builders<Event<TMessageId, TEntityKey>>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.TriggeredByUser)
            .Ascending(x => x.MessageOnUtc);

        await indexes.CreateOneAsync(
            new CreateIndexModel<Event<TMessageId, TEntityKey>>(keyIndex, new CreateIndexOptions { Unique = false }),
            null,
            cancellationToken
        );
    }
}
