namespace SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds;

public interface IQueryMongoDbRepository :
    IQueryMongoDbRepository<Guid>,
    IQueryRepository,
    IQueryHandlerRepository
{
}

public class QueryRepository(
    IMongoDatabase database, IIdGenerator idGenerator,
    ILogger logger, InsertOneOptions? insertOneOptions = null
) :
    QueryRepository<Guid>(database, idGenerator, logger, insertOneOptions),
    IQueryMongoDbRepository
{
}
