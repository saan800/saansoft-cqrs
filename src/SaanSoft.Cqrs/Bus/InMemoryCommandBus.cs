using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryCommandBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryCommandBus<Guid>(serviceProvider, logger);

public abstract class InMemoryCommandBus<TMessageId>(IServiceProvider serviceProvider, ILogger logger)
    : ICommandPublisher<TMessageId>,
      ICommandSubscriber<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task ExecuteAsync<TCommand>(TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        // get subscriber via ServiceProvider so it runs through any decorators
        var subscriber = ServiceProvider.GetRequiredService<ICommandSubscriber<TMessageId>>();
        await subscriber.RunAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        // get subscriber via ServiceProvider so it runs through any decorators
        var subscriber = ServiceProvider.GetRequiredService<ICommandSubscriber<TMessageId>>();
        await subscriber.RunAsync(command, cancellationToken);
    }

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<ICommandHandler<TCommand>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                if (command.IsReplay) return;

                var handler = handlers.Single();
                Logger.LogInformation("Running command handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TCommand).FullName);
                await handler.HandleAsync(command, cancellationToken);
                return;
            case 0:
                throw new InvalidOperationException($"No handler for type '{typeof(ICommandHandler<TCommand>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
                    throw new InvalidOperationException($"Only one handler for type '{typeof(ICommandHandler<TCommand>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
