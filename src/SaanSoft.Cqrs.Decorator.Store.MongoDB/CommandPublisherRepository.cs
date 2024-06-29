namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandPublisherMongoDbRepository<TMessageId> :
    ICommandPublisherRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class CommandPublisherRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    CommandPublisherRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class CommandPublisherRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BasePublisherRepository<TMessageId, ICommand<TMessageId>>(database, idGenerator, replaceOptions),
    ICommandPublisherMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "CommandPublishers";
}
