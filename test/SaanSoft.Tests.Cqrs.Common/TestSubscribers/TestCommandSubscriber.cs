using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestSubscribers;

/// <summary>
/// Test command subscriber bus -  useful when testing decorators
/// </summary>
/// <param name="serviceProvider"></param>
public class TestCommandSubscriber(IServiceProvider serviceProvider) : ICommandSubscriber<Guid>
{
    public Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<Guid>
        => GetHandler<TCommand>().HandleAsync(command, cancellationToken);

    public ICommandHandler<TCommand> GetHandler<TCommand>() where TCommand : ICommand<Guid>
    {
        var handlers = serviceProvider.GetServices<ICommandHandler<TCommand>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
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
