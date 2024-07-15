namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository<TMessageId> :
    ICommandRepository<TMessageId>,
    ICommandHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<BaseCommand<TMessageId>> MessageCollection { get; }
}

public abstract class CommandRepository<TMessageId>(
        IMongoDatabase database, IIdGenerator<TMessageId> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
        ) :
    BaseMessageRepository<TMessageId, IRootCommand<TMessageId>>(database, idGenerator, logger, insertOneOptions),
    ICommandMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "CommandMessages";

    public IMongoCollection<BaseCommand<TMessageId>> MessageCollection
        => Database.GetCollection<BaseCommand<TMessageId>>(CollectionName);

    public override async Task UpsertHandlerAsync(TMessageId id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<BaseCommand<TMessageId>>.Filter.Eq(x => x.Id, id);
        var metadata = (await MessageCollection
            .Find(filter)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync(cancellationToken));

        if (metadata == null)
        {
            Logger.LogError(
                "The command {Id} could not be found when attempting to add handler metadata for {HandlerType}",
                id.ToString(),
                messageHandler.TypeFullName
            );
            return;
        }

        metadata.AddHandlerMetadata(messageHandler);

        await MessageCollection.FindOneAndUpdateAsync(
            filter,
            Builders<BaseCommand<TMessageId>>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the MessageCollection
    /// </summary>
    public override async Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<BaseCommand<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageOnUtc);

        var indexModel =
            new CreateIndexModel<BaseCommand<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = false, Background = true });

        await MessageCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}
