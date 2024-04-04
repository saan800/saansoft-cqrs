using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Process the command including validation and other business logic to ensure its valid to continue.
    /// Command handling should not alter any state in the DB.
    /// The handler will often raise one or more associated events.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>If the command was successful or not, and an error message if applicable.</returns>
    Task<CommandResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
