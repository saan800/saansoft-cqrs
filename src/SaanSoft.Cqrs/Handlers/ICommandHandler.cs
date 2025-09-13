namespace SaanSoft.Cqrs.Handlers;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Process the command including validation and other business logic to ensure its valid to continue.
    /// Command handling should not alter any state in the system.
    /// The handler will often raise one or more associated events.
    /// </summary>
    Task HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Process the command including validation and other business logic to ensure its valid to continue.
    /// Command handling should not alter any state in the DB.
    /// The handler will often raise one or more associated events.
    ///
    /// Returns the result of the command
    /// </summary>
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
