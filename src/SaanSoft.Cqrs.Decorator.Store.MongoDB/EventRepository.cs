namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventMongoDbRepository : IEventMongoDbRepository<Guid>
{
}

public interface IEventMongoDbRepository<TEntityKey> : IEventMongoDbRepository<Guid, TEntityKey>
    where TEntityKey : struct
{
}

public interface IEventMongoDbRepository<TMessageId, TEntityKey> :
    IEventRepository<TMessageId, TEntityKey>,
    IEventHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
    where TEntityKey : struct
{
    IMongoCollection<Event<TMessageId, TEntityKey>> MessageCollection { get; }
}

public class EventRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ILogger logger, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid>(database, idGenerator, logger, insertOneOptions),
      IEventMongoDbRepository
{
}

public class EventRepository<TEntityKey>(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ILogger logger, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid, TEntityKey>(database, idGenerator, logger, insertOneOptions),
      IEventMongoDbRepository<TEntityKey>
    where TEntityKey : struct
{
}

/// <summary>
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public abstract class EventRepository<TMessageId, TEntityKey>(
        IMongoDatabase database, IIdGenerator<TMessageId> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    ) :
    BaseMessageRepository<TMessageId, IEvent<TMessageId>>(database, idGenerator, logger, insertOneOptions),
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

    public override async Task UpsertHandlerAsync(TMessageId id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<Event<TMessageId, TEntityKey>>.Filter.Eq(x => x.Id, id);
        var metadata = (await MessageCollection
            .Find(filter)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync(cancellationToken));

        if (metadata == null)
        {
            Logger.LogError(
                "The event {Id} could not be found when attempting to add handler metadata for {HandlerType}",
                id.ToString(),
                messageHandler.TypeFullName
            );
            return;
        }

        metadata.AddHandlerMetadata(messageHandler);

        await MessageCollection.FindOneAndUpdateAsync(
            filter,
            Builders<Event<TMessageId, TEntityKey>>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = MessageCollection.Indexes;

        var keyIndex = Builders<Event<TMessageId, TEntityKey>>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.MessageOnUtc)
            .Ascending(x => x.Metadata.TypeFullName);

        await indexes.CreateOneAsync(
            new CreateIndexModel<Event<TMessageId, TEntityKey>>(keyIndex, new CreateIndexOptions { Unique = false }),
            null,
            cancellationToken
        );
    }
}
