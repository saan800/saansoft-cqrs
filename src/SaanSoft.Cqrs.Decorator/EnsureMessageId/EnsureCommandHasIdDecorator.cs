namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Command has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="next"></param>
public class EnsureCommandHasIdDecorator(ICommandBus next) :
    ICommandBusDecorator
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = Guid.NewGuid();
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        if (GenericUtils.IsNullOrDefault(typedCommand.Id)) typedCommand.Id = Guid.NewGuid();
        return await next.ExecuteAsync(typedCommand, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = Guid.NewGuid();
        await next.ExecuteAsync(command, cancellationToken);
    }
}
