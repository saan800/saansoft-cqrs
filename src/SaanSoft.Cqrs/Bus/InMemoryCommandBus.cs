using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Bus;

public abstract class InMemoryCommandBus<TMessageId>(IServiceProvider serviceProvider, IIdGenerator<TMessageId> idGenerator, ILogger logger)
    : ICommandBus<TMessageId>,
      ICommandSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly IIdGenerator<TMessageId> IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = IdGenerator.NewId();

        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<ICommandSubscriptionBus<TMessageId>>();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        if (GenericUtils.IsNullOrDefault(typedCommand.Id)) typedCommand.Id = IdGenerator.NewId();

        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<ICommandSubscriptionBus<TMessageId>>();
        return await subscriptionBus.RunAsync(typedCommand, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = IdGenerator.NewId();

        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<ICommandSubscriptionBus<TMessageId>>();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        Logger.LogInformation("Running command handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, command.TypeFullName);
        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        Logger.LogInformation("Running command handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, command.TypeFullName);
        return await handler.HandleAsync((TCommand)command, cancellationToken);
    }

    public virtual ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : ICommand<TMessageId>
        => GetCommandHandler<ICommandHandler<TCommand>>();

    public ICommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
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
                    var typeNames = handlers.Select(handler => handler!.GetType().FullName ?? handler!.GetType().Name).ToList();
                    throw new InvalidOperationException($"Only one handler for type '{typeof(TCommandHandler)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
