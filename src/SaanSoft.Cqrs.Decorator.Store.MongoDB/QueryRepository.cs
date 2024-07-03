namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbRepository<TMessageId> :
    IQueryRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<Query<TMessageId>> MessageCollection { get; }
}

public class QueryRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertOneOptions? insertOneOptions = null) :
    QueryRepository<Guid>(database, idGenerator, insertOneOptions)
{
}

public abstract class QueryRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, InsertOneOptions? insertOneOptions = null) :
    BaseMessageRepository<TMessageId, IQuery<TMessageId>>(database, idGenerator, insertOneOptions),
    IQueryMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "QueryMessages";

    public IMongoCollection<Query<TMessageId>> MessageCollection
        => Database.GetCollection<Query<TMessageId>>(CollectionName);
}
