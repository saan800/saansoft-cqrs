namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandDecorator(ICommandRepository repository, ICommandBus next) : ICommandBus
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await StoreMessageAsync(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        await StoreMessageAsync(command, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await StoreMessageAsync(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    private async Task StoreMessageAsync(IBaseCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsReplay)
        {
            await repository.InsertAsync(message, cancellationToken);
        }
    }
}
