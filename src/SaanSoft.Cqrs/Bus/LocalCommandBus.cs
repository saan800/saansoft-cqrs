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
        return await serviceProvider
            .GetRequiredService<ICommandHandler<TCommand>>()
            .HandleAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
        => await ExecuteAsync(command, cancellationToken);
}
