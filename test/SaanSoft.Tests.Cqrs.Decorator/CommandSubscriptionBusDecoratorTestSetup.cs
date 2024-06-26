namespace SaanSoft.Tests.Cqrs.Decorator;

public abstract class CommandSubscriptionBusDecoratorTestSetup : TestSetup
{
    protected CommandSubscriptionBusDecoratorTestSetup()
    {
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<AnotherCommand>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<MyCommandWithResponse, string>, CommandHandler>();
        ServiceCollection.AddScoped<ICommandHandler<AnotherCommandWithResponse, string>, CommandHandler>();
    }

    protected abstract ICommandSubscriptionBusDecorator<Guid> SutSubscriptionBusDecorator { get; }
}
