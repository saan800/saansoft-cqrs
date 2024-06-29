namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryPublisherMongoDbRepository<TMessageId> :
    IQueryPublisherRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class QueryPublisherRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    QueryPublisherRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class QueryPublisherRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BasePublisherRepository<TMessageId, IQuery<TMessageId>>(database, idGenerator, replaceOptions),
    IQueryPublisherMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "QueryPublishers";
}
