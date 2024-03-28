using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<CommandResult> HandleAsync(TCommand command);
}
