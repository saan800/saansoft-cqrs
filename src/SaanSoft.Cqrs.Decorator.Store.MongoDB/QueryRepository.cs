namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbRepository :
    IQueryRepository,
    IMongoDbRepository
{
    IMongoCollection<Query> MessageCollection { get; }
}

public class QueryRepository(
        IMongoDatabase database,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    ) :
    BaseMessageRepository<IQuery>(database, logger, insertOneOptions),
    IQueryMongoDbRepository
{
    public override string CollectionName => "QueryMessages";

    public IMongoCollection<Query> MessageCollection
        => Database.GetCollection<Query>(CollectionName);

    public override async Task<IQuery?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
        => await MessageCollection
            .Find(x => x.Id == messageId)
            .FirstOrDefaultAsync(cancellationToken);

    public override async Task UpsertHandlerAsync(Guid id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<Query>.Filter.Eq(x => x.Id, id);
        var metadata = (await MessageCollection
            .Find(filter)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync(cancellationToken));

        if (metadata == null)
        {
            Logger.LogError(
                "The query {Id} could not be found when attempting to add handler metadata for {HandlerType}",
                id.ToString(),
                messageHandler.TypeFullName
            );
            return;
        }

        metadata.AddHandlerMetadata(messageHandler);

        await MessageCollection.FindOneAndUpdateAsync(
            filter,
            Builders<Query>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the MessageCollection
    /// </summary>
    public override async Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<Query>.IndexKeys
            .Ascending(x => x.MessageOnUtc);

        var indexModel =
            new CreateIndexModel<Query>(keyIndex, new CreateIndexOptions { Unique = false, Background = false });

        await MessageCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}
