namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbRepository<TMessageId> :
    IQueryRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<Query<TMessageId>> MessageCollection { get; }
}

public class QueryRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, InsertManyOptions? insertManyOptions = null) :
    QueryRepository<Guid>(database, idGenerator, insertManyOptions)
{
}

public abstract class QueryRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, InsertManyOptions? insertManyOptions = null) :
    BaseMessageRepository<TMessageId, IQuery<TMessageId>>(database, idGenerator, insertManyOptions),
    IQueryMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "QueryMessages";

    public IMongoCollection<Query<TMessageId>> MessageCollection
        => Database.GetCollection<Query<TMessageId>>(CollectionName);
}
