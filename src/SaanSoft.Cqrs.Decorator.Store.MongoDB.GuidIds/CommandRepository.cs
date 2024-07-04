namespace SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds;

public interface ICommandMongoDbRepository :
    ICommandMongoDbRepository<Guid>,
    ICommandRepository,
    ICommandHandlerRepository
{
}

public class CommandRepository(
    IMongoDatabase database, IIdGenerator idGenerator,
    ILogger logger, InsertOneOptions? insertOneOptions = null
)
    : CommandRepository<Guid>(database, idGenerator, logger, insertOneOptions),
      ICommandMongoDbRepository
{
}
