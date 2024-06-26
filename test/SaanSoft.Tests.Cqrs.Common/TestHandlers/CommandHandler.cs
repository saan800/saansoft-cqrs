namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class CommandHandler :
    ICommandHandler<MyCommand>,
    ICommandHandler<AnotherCommand>,
    ICommandHandler<MyCommandWithResponse, string>,
    ICommandHandler<AnotherCommandWithResponse, string>
{
    public Task HandleAsync(MyCommand command, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task HandleAsync(AnotherCommand command, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<string> HandleAsync(MyCommandWithResponse command, CancellationToken cancellationToken = default)
        => Task.FromResult(command.Message);

    public Task<string> HandleAsync(AnotherCommandWithResponse command, CancellationToken cancellationToken = default)
        => Task.FromResult(command.Message);
}
