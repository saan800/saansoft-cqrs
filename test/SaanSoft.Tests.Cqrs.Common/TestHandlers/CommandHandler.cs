using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestModels;

namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class CommandHandler :
    ICommandHandler<MyCommand>,
    ICommandHandler<AnotherCommand>
{
    public Task HandleAsync(MyCommand command, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task HandleAsync(AnotherCommand command, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
