using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.Common.TestModels;

namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class CommandHandler :
    ICommandHandler<MyCommand>,
    ICommandHandler<AnotherCommand>
{
    public Task<CommandResponse> HandleAsync(MyCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandResponse());

    public Task<CommandResponse> HandleAsync(AnotherCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommandResponse());
}
