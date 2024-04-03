using Microsoft.Extensions.DependencyInjection;

using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class LocalCommandBus(IServiceProvider serviceProvider)
    : LocalCommandBus<Guid>(serviceProvider);

public abstract class LocalCommandBus<TMessageId>(IServiceProvider serviceProvider)
    : ICommandBus<TMessageId>
    where TMessageId : struct
{
    public async Task<CommandResult> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handlers = serviceProvider.GetServices<ICommandHandler<TCommand>>()?.ToList() ?? [];
        if (handlers.Count == 1)
        {
            return await handlers.First().HandleAsync(command, cancellationToken);
        }
        if (handlers.Count == 0)
        {
            throw new InvalidOperationException($"No service for type '{typeof(ICommandHandler<TCommand>)}' has been registered");
        }

        var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
        throw new InvalidOperationException($"Only one service for type '{typeof(ICommandHandler<TCommand>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
        => await ExecuteAsync(command, cancellationToken);
}
