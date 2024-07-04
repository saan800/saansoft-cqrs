namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandHandlerRepositoryTests : BaseHandlerRepositoryTests<MyCommand, CommandRepository, BaseCommand<Guid>>
{
    public CommandHandlerRepositoryTests()
    {
        SutRepository = new CommandRepository(Database, IdGenerator, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyCommand CreateNewMessage() => new MyCommand();
}
