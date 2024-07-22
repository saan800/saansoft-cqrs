namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventMongoDbRepository<TEntityKey> :
    IEventRepository<TEntityKey>,
    IEventHandlerRepository,
    IMongoDbRepository
    where TEntityKey : struct
{
    IMongoCollection<Event<TEntityKey>> MessageCollection { get; }
}

/// <summary>
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
/// <typeparam name="TEntityKey"></typeparam>
public class EventRepository<TEntityKey>(
        IMongoDatabase database,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    ) :
    BaseMessageRepository<IEvent>(database, logger, insertOneOptions),
    IEventMongoDbRepository<TEntityKey>
    where TEntityKey : struct
{
    public override string CollectionName => "EventMessages";

    public virtual IMongoCollection<Event<TEntityKey>> MessageCollection
        => Database.GetCollection<Event<TEntityKey>>(CollectionName);

    public async Task<List<IEvent<TEntityKey>>> GetEntityMessagesAsync(TEntityKey key,
        CancellationToken cancellationToken = default)
        => (await MessageCollection
            .Find(x => x.Key.Equals(key))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.MessageOnUtc)
            .Select(x => (IEvent<TEntityKey>)x)
            .ToList();

    public override async Task UpsertHandlerAsync(Guid id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<Event<TEntityKey>>.Filter.Eq(x => x.Id, id);
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
            Builders<Event<TEntityKey>>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default)
    {
        var indexes = MessageCollection.Indexes;

        var keyIndex = Builders<Event<TEntityKey>>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.MessageOnUtc);

        await indexes.CreateOneAsync(
            new CreateIndexModel<Event<TEntityKey>>(keyIndex, new CreateIndexOptions { Unique = false }),
            null,
            cancellationToken
        );
    }
}


/// <summary>
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
public class EventRepository(
    IMongoDatabase database,
    ILogger logger,
    InsertOneOptions? insertOneOptions = null
) :
    EventRepository<Guid>(database, logger, insertOneOptions),
    IEventRepository
{
}
