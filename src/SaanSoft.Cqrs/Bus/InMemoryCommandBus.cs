using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider)
    : ICommandBus,
      ICommandSubscriptionBus
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = Guid.NewGuid();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        if (GenericUtils.IsNullOrDefault(typedCommand.Id)) typedCommand.Id = Guid.NewGuid();

        var subscriptionBus = GetSubscriptionBus();
        return await subscriptionBus.RunAsync(typedCommand, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = Guid.NewGuid();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual ICommandSubscriptionBus GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<ICommandSubscriptionBus>();

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        var handler = GetHandler<TCommand>();
        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        return await handler.HandleAsync(typedCommand, cancellationToken);
    }

    public virtual ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, ICommand
        => GetCommandHandler<ICommandHandler<TCommand>>();

    public ICommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, ICommand<TCommand, TResponse>
        => GetCommandHandler<ICommandHandler<TCommand, TResponse>>();

    private TCommandHandler GetCommandHandler<TCommandHandler>()
    {
        var handlers = ServiceProvider.GetServices<TCommandHandler>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
            case 0:
                throw new InvalidOperationException($"No handler for type '{typeof(TCommandHandler)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(handler => handler!.GetType().GetTypeFullName()).ToList();
                    throw new InvalidOperationException($"Only one handler for type '{typeof(TCommandHandler)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
