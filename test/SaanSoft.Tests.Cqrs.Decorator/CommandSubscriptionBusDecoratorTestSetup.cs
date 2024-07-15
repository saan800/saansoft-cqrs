using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class CommandSubscriptionBusTestSetup : TestSetup
{
    protected CommandSubscriptionBusTestSetup()
    {
        ServiceCollection.AddScoped<IBaseCommandHandler<MyCommand>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<AnotherCommand>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<MyCommandWithResponse, string>, CommandHandler>();
        ServiceCollection.AddScoped<IBaseCommandHandler<AnotherCommandWithResponse, string>, CommandHandler>();
    }

    protected abstract ICommandSubscriptionBus SutSubscriptionBus { get; }
}
