using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface ICommandSubscriber<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Runs a command and waits for a CommandResult.
    /// Any issues in the command will result in an exception being thrown.
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>;
}
