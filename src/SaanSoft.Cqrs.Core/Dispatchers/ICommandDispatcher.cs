using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Dispatchers;

public interface ICommandDispatcher
{
    /// <summary>
    /// Execute a command and wait for a CommandResult, which indicates if it errored or not and an error message.
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    Task<CommandResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;

    /// <summary>
    /// Put the command onto the queue.
    /// It will not have any indication if the command was successfully executed or not
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TCommand>(TCommand command) where TCommand : ICommand;
}
