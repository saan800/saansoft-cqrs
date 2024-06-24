using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository<TMessageId> : ICommandRepository<TMessageId>, IMongoDbStore<TMessageId>
    where TMessageId : struct
{
}

public class CommandRepository(IMongoDatabase database)
    : CommandRepository<Guid>(database)
{
    protected override Guid NewMessageId() => Guid.NewGuid();
}

public abstract class CommandRepository<TMessageId>(IMongoDatabase database) :
    BaseMessageRepository<TMessageId, ICommand<TMessageId>>(database),
    ICommandMongoDbRepository<TMessageId>,
    ICommandPublisherRepository<TMessageId>,
    ICommandHandlerRepository<TMessageId>
    where TMessageId : struct
{
    public override string MessageCollectionName => "CommandMessages";
    public override string PublisherCollectionName => "CommandPublishers";
    public override string HandlerCollectionName => "CommandHandlers";
}
