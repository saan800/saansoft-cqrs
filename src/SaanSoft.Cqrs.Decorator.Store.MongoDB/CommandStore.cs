using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbStore<TMessageId> : ICommandStore<TMessageId>, IMongoDbStore<TMessageId>
    where TMessageId : struct
{
}

public class CommandStore(IMongoDatabase database)
    : CommandStore<Guid>(database)
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

public abstract class CommandStore<TMessageId>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, ICommand<TMessageId>>(database),
    ICommandMongoDbStore<TMessageId>,
    ICommandPublisherStore,
    ICommandSubscriberStore
    where TMessageId : struct
{
    public override string MessageCollectionName => "CommandMessages";
    public override string PublisherCollectionName => "CommandPublishers";
    public override string SubscriberCollectionName => "CommandSubscribers";
}
