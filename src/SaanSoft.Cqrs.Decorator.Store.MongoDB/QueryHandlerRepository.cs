namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryHandlerMongoDbRepository<TMessageId> :
    IQueryHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class QueryHandlerRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    QueryHandlerRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class QueryHandlerRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BaseHandlerRepository<TMessageId, IQuery<TMessageId>>(database, idGenerator, replaceOptions),
    IQueryHandlerMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "QueryHandlers";
}

