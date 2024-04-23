using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public class CommandStore(IMongoDatabase database)
    : CommandStore<Guid>(database)
{
}

public abstract class CommandStore<TMessageId>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, ICommand<TMessageId>>(database),
    ICommandStore<TMessageId>
    where TMessageId : struct
{
    public override string MessageCollectionName => "Commands";
}
