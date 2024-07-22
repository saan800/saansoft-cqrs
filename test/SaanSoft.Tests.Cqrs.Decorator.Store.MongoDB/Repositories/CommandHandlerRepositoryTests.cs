namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class CommandHandlerRepositoryTests : BaseHandlerRepositoryTests<IBaseCommand, CommandRepository>
{
    public CommandHandlerRepositoryTests()
    {
        SutRepository = new CommandRepository(Database, Logger);
    }

    protected override MyCommand CreateNewMessage() => new MyCommand();
}
