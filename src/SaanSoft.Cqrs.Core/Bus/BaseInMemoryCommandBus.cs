using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Core.Utilities;

namespace SaanSoft.Cqrs.Core.Bus;

public abstract class BaseInMemoryCommandBus<TMessageId>(IServiceProvider serviceProvider, IIdGenerator<TMessageId> idGenerator)
    : IBaseCommandBus<TMessageId>,
      IBaseCommandSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly IIdGenerator<TMessageId> IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = IdGenerator.NewId();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        if (GenericUtils.IsNullOrDefault(typedCommand.Id)) typedCommand.Id = IdGenerator.NewId();

        var subscriptionBus = GetSubscriptionBus();
        return await subscriptionBus.RunAsync(typedCommand, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = IdGenerator.NewId();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(command, cancellationToken);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual IBaseCommandSubscriptionBus<TMessageId> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IBaseCommandSubscriptionBus<TMessageId>>();

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        return await handler.HandleAsync(typedCommand, cancellationToken);
    }

    public virtual IBaseCommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, IBaseCommand<TMessageId>
        => GetCommandHandler<IBaseCommandHandler<TCommand>>();

    public IBaseCommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
        => GetCommandHandler<IBaseCommandHandler<TCommand, TResponse>>();

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
