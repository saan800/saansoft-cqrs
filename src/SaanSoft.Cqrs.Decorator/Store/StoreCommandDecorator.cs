namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreCommandDecorator<TMessageId>(ICommandRepository<TMessageId> repository, ICommandBus<TMessageId> next)
    : BaseStoreMessageDecorator<TMessageId, IRootCommand<TMessageId>>(repository),
      ICommandBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        await StoreMessageAsync(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        await StoreMessageAsync((TCommand)command, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        await StoreMessageAsync(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }
}
