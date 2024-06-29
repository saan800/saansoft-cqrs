namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventMongoDbRepository<TMessageId, TEntityKey> :
    IEventRepository<TMessageId, TEntityKey>,
    IMongoDbRepository
    where TMessageId : struct
    where TEntityKey : struct
{
    IMongoCollection<IEvent<TMessageId, TEntityKey>> EventCollection { get; }
    IMongoCollection<IMessage<TMessageId>> MessageCollection { get; }
}

public class EventRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertManyOptions? insertManyOptions = null)
    : EventRepository<Guid>(database, idGenerator, insertManyOptions)
{
}

public class EventRepository<TEntityKey>(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertManyOptions? insertManyOptions = null)
    : EventRepository<Guid, TEntityKey>(database, idGenerator, insertManyOptions)
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
public abstract class EventRepository<TMessageId, TEntityKey>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, InsertManyOptions? insertManyOptions = null) :
    BaseMessageRepository<TMessageId, IEvent<TMessageId>>(database, idGenerator, insertManyOptions),
    IEventMongoDbRepository<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public override string CollectionName => "EventMessages";

    public IMongoCollection<IEvent<TMessageId, TEntityKey>> EventCollection
        => Database.GetCollection<IEvent<TMessageId, TEntityKey>>(CollectionName);

    public IMongoCollection<IMessage<TMessageId>> MessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(CollectionName);

    public async Task<List<IEvent<TMessageId, TEntityKey>>> GetEntityMessagesAsync(TEntityKey key,
        CancellationToken cancellationToken = default)
        => (await EventCollection
            .Find(x => x.Key.Equals(key))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.MessageOnUtc)
            .ToList();

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = EventCollection.Indexes;

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
