
using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class CommandBusTestSetup : TestSetup
{
    protected CommandBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseCommandHandler<MyCommand>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<AnotherCommand>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<MyCommandWithResponse, string>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<AnotherCommandWithResponse, string>, CommandHandler>();
    }

    protected abstract ICommandBus SutPublisherDecorator { get; }
}
