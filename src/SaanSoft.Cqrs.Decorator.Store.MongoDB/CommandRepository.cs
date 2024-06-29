namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository<TMessageId> :
    ICommandRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<IMessage<TMessageId>> MessageCollection { get; }
}

public class CommandRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator)
    : CommandRepository<Guid>(database, idGenerator)
{
}

public abstract class CommandRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator) :
    BaseMessageRepository<TMessageId, ICommand<TMessageId>>(database, idGenerator),
    ICommandMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "CommandMessages";

    public IMongoCollection<IMessage<TMessageId>> MessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(CollectionName);
}
