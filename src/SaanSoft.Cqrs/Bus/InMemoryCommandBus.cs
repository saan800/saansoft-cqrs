using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider, ILogger logger, CommandBusOptions? options = null)
    : InMemoryCommandBus<Guid>(serviceProvider, logger, options);

public abstract class InMemoryCommandBus<TMessageId>
    : ICommandBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly CommandBusOptions Options;
    protected readonly LogLevel LogLevel;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;
    // ReSharper restore MemberCanBePrivate.Global

    protected InMemoryCommandBus(IServiceProvider serviceProvider, ILogger logger, CommandBusOptions? options = null)
    {
        Options = options ?? new CommandBusOptions();
        LogLevel = Options.LogLevel;

        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CommandResponse> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<ICommandHandler<TCommand>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                var handler = handlers.Single();

                if (Logger.IsEnabled(LogLevel))
                {
                    Logger.Log(LogLevel, "Running command handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TCommand).FullName);
                }
                return await handler.HandleAsync(command, cancellationToken);
            case 0:
                throw new InvalidOperationException($"No service for type '{typeof(ICommandHandler<TCommand>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
                    throw new InvalidOperationException($"Only one service for type '{typeof(ICommandHandler<TCommand>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
        => await ExecuteAsync(command, cancellationToken);
}
