namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface ICommandHandlerMongoDbRepository<TMessageId> :
    ICommandHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class CommandHandlerRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    CommandHandlerRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class CommandHandlerRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BaseHandlerRepository<TMessageId, ICommand<TMessageId>>(database, idGenerator, replaceOptions),
    ICommandHandlerMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "CommandHandlers";
}
