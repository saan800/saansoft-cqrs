using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public class QueryStore(IMongoDatabase database) : QueryStore<Guid>(database)
{
}

public abstract class QueryStore<TMessageId>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, IQuery<TMessageId>>(database),
    IQueryStore<TMessageId>
    where TMessageId : struct
{
    public override string MessageCollectionName => "Queries";
}
