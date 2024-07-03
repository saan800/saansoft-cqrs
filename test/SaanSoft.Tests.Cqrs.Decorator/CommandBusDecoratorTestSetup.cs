
namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class CommandBusDecoratorTestSetup : TestSetup
{
    protected CommandBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<AnotherCommand>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<MyCommandWithResponse, string>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<AnotherCommandWithResponse, string>, CommandHandler>();
    }

    protected abstract ICommandBusDecorator SutPublisherDecorator { get; }
}
