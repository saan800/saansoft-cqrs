namespace SaanSoft.Cqrs.Decorator.Store;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreCommandPublisherDecorator<TMessageId>(ICommandPublisherRepository<TMessageId> repository, ICommandBus<TMessageId> next) :
      BaseStoreMessagePublisherDecorator<TMessageId, ICommand<TMessageId>>(repository),
      ICommandBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<ICommandBus<TMessageId>>(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        await StorePublisher<ICommandBus<TMessageId>>(typedCommand, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<ICommandBus<TMessageId>>(command, cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }
}
