using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface ICommandBus<TMessageId>
{
    /// <summary>
    /// Execute a command and wait for a CommandResult
    /// The CommandResult indicates if it errored or not and an error message.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    Task<CommandResult> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TMessageId>;

    /// <summary>
    /// Put the command onto the queue (i.e. fire and forget)
    /// It will not return any indication if the command was successfully executed or not
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TMessageId>;
}
