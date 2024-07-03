namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandMongoDbRepository<TMessageId> :
    ICommandRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
    IMongoCollection<BaseCommand<TMessageId>> MessageCollection { get; }
}

public class CommandRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator)
    : CommandRepository<Guid>(database, idGenerator)
{
}

public abstract class CommandRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator) :
    BaseMessageRepository<TMessageId, IBaseCommand<TMessageId>>(database, idGenerator),
    ICommandMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "CommandMessages";

    public IMongoCollection<BaseCommand<TMessageId>> MessageCollection
        => Database.GetCollection<BaseCommand<TMessageId>>(CollectionName);
}
