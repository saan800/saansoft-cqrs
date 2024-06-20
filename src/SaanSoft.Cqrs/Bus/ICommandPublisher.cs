using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface ICommandPublisher<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Execute a command and wait for a CommandResult.
    /// Any issues in the command will result in an exception being thrown.
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <remarks>
    /// Use ExecuteAsync if you need to wait for the command to finish processing before continuing.
    ///
    /// It is recommended that ExecuteAsync is used for commands that are handled within the same application
    /// and doesn't depend on external infrastructure (e.g. using InMemoryCommandBus).
    /// </remarks>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>;

    /// <summary>
    /// Put the command onto the queue (i.e. fire and forget).
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <remarks>
    /// Use QueueAsync if the command could be handled out of scope.
    ///
    /// Its recommended that QueueAsync is used for commands that are handled via external infrastructure
    /// (e.g. Aws SNS/SQS, Azure Service Bus, RabbitMQ).
    /// </remarks>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>;
}
