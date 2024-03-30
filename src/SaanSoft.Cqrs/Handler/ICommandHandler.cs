using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface ICommandHandler
{
    Task<CommandResult> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;
}
