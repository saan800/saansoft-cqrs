namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository : ICommandMongoDbRepository<Guid>
{
}

public interface ICommandMongoDbRepository<TMessageId> :
    ICommandRepository<TMessageId>,
    ICommandHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<BaseCommand<TMessageId>> MessageCollection { get; }
}

public class CommandRepository(
        IMongoDatabase database, IIdGenerator<Guid> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
    )
    : CommandRepository<Guid>(database, idGenerator, logger, insertOneOptions),
      ICommandMongoDbRepository
{
}

public abstract class CommandRepository<TMessageId>(
        IMongoDatabase database, IIdGenerator<TMessageId> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
        ) :
    BaseMessageRepository<TMessageId, IBaseCommand<TMessageId>>(database, idGenerator, logger, insertOneOptions),
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
}
