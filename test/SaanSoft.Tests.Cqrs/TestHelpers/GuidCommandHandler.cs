using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidCommandHandler : ICommandHandler<GuidCommand>
{
    public Task<CommandResult> HandleAsync(GuidCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CommandResult());
    }
}
