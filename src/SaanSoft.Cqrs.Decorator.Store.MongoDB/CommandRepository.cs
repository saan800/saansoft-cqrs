namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository :
    ICommandRepository,
    IMongoDbRepository
{
    IMongoCollection<BaseCommand> MessageCollection { get; }
}

public class CommandRepository(
        IMongoDatabase database,
        ILogger logger, InsertOneOptions? insertOneOptions = null
        ) :
    BaseMessageRepository<IBaseCommand>(database, logger, insertOneOptions),
    ICommandMongoDbRepository
{
    public override string CollectionName => "CommandMessages";

    public IMongoCollection<BaseCommand> MessageCollection
        => Database.GetCollection<BaseCommand>(CollectionName);

    public override async Task<IBaseCommand?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
        => await MessageCollection
            .Find(x => x.Id == messageId)
            .FirstOrDefaultAsync(cancellationToken);

    public override async Task UpsertHandlerAsync(Guid id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<BaseCommand>.Filter.Eq(x => x.Id, id);
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
            Builders<BaseCommand>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the MessageCollection
    /// </summary>
    public override async Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<BaseCommand>.IndexKeys
            .Ascending(x => x.MessageOnUtc);

        var indexModel =
            new CreateIndexModel<BaseCommand>(keyIndex, new CreateIndexOptions { Unique = false, Background = true });

        await MessageCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}
