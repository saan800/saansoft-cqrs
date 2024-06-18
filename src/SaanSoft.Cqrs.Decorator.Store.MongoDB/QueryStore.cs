using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IQueryMongoDbStore<TMessageId> : IQueryStore<TMessageId>, IMongoDbStore<TMessageId>
    where TMessageId : struct
{
}

public class QueryStore(IMongoDatabase database) : QueryStore<Guid>(database)
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

public abstract class QueryStore<TMessageId>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, IQuery<TMessageId>>(database),
    IQueryMongoDbStore<TMessageId>,
    IQueryPublisherStore,
    IQuerySubscriberStore
    where TMessageId : struct
{
    public override string MessageCollectionName => "QueryMessages";
    public override string PublisherCollectionName => "QueryPublishers";
    public override string SubscriberCollectionName => "QuerySubscribers";
}
