namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbRepository : IQueryMongoDbRepository<Guid>
{
}

public interface IQueryMongoDbRepository<TMessageId> :
    IQueryRepository<TMessageId>,
    IQueryHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<Query<TMessageId>> MessageCollection { get; }
}

public class QueryRepository(
        IMongoDatabase database, IIdGenerator<Guid> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    ) :
    QueryRepository<Guid>(database, idGenerator, logger, insertOneOptions),
    IQueryMongoDbRepository
{
}

public abstract class QueryRepository<TMessageId>(
        IMongoDatabase database, IIdGenerator<TMessageId> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    ) :
    BaseMessageRepository<TMessageId, IQuery<TMessageId>>(database, idGenerator, logger, insertOneOptions),
    IQueryMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "QueryMessages";

    public IMongoCollection<Query<TMessageId>> MessageCollection
        => Database.GetCollection<Query<TMessageId>>(CollectionName);

    public override async Task UpsertHandlerAsync(TMessageId id, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageHandler = handlerType.BuildMessageHandler(exception);

        var filter = Builders<Query<TMessageId>>.Filter.Eq(x => x.Id, id);
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
            Builders<Query<TMessageId>>.Update.Set(x => x.Metadata, metadata),
            cancellationToken: cancellationToken);
    }
}
