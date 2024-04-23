using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface ICommandSubscriber<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Runs a command and waits for a CommandResult.
    /// The CommandResult indicates if it errored or not and an error message.
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns>
    /// Contains information on if the query was successful or not, and error messages.
    /// </returns>
    Task<CommandResponse> RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TMessageId>;
}
