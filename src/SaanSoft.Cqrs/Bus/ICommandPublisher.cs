using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface ICommandPublisher<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Execute a command and wait for a CommandResult.
    /// The CommandResult indicates if it errored or not and an error message.
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <remarks>
    /// Use ExecuteAsync if you need to wait for the command to finish processing before continuing.
    ///
    /// Its recommended that ExecuteAsync is used for commands that are handled within the same application
    /// and doesn't depend on external infrastructure (e.g. using InMemoryCommandBus).
    /// </remarks>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns>
    /// Contains information on if the query was successful or not, and error messages.
    /// </returns>
    Task<CommandResponse> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TMessageId>;

    /// <summary>
    /// Put the command onto the queue (i.e. fire and forget).
    /// It will not return any indication if the command was successfully executed or not.
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <remarks>
    /// Use QueueAsync if the command could be handled out of scope.
    ///
    /// Its recommended that QueueAsync is used for commands that are handled via external infrastructure
    /// (e.g. Aws SQS, Azure Service Bus, RabbitMQ).
    /// </remarks>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TMessageId>;
}
