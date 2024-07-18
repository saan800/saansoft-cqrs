namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class CommandHandlerRepositoryTests : BaseHandlerRepositoryTests<MyCommand, CommandRepository, BaseCommand>
{
    public CommandHandlerRepositoryTests()
    {
        SutRepository = new CommandRepository(Database, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyCommand CreateNewMessage() => new MyCommand();
}
