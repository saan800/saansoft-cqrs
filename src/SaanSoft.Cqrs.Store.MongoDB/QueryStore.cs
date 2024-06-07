using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public class QueryStore(IMongoDatabase database) : QueryStore<Guid>(database)
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

public abstract class QueryStore<TMessageId>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, IQuery<TMessageId>>(database),
    IQueryPublisherStore,
    IQueryStore<TMessageId>
    where TMessageId : struct
{
    public override string MessageCollectionName => "Queries";
    public override string PublisherCollectionName => "QueryPublishers";
}
