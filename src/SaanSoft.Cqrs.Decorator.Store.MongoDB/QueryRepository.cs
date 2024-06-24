using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbRepository<TMessageId> : IQueryRepository<TMessageId>, IMongoDbStore<TMessageId>
    where TMessageId : struct
{
}

public class QueryRepository(IMongoDatabase database) : QueryRepository<Guid>(database)
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

public abstract class QueryRepository<TMessageId>(IMongoDatabase database) :
    BaseMessageRepository<TMessageId, IQuery<TMessageId>>(database),
    IQueryMongoDbRepository<TMessageId>,
    IQueryPublisherRepository<TMessageId>,
    IQueryHandlerRepository<TMessageId>
    where TMessageId : struct
{
    public override string MessageCollectionName => "QueryMessages";
    public override string PublisherCollectionName => "QueryPublishers";
    public override string HandlerCollectionName => "QueryHandlers";
}
