namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the command's metadata.
///
/// Should be used in conjunction with <see cref="StoreCommandDecorator{TMessageId}"/>
/// </summary>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreCommandPublisherDecorator<TMessageId>(ICommandBus<TMessageId> next) :
      BaseStoreMessagePublisherDecorator<TMessageId>,
      ICommandBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        await StorePublisherAsync<ICommandBus<TMessageId>>(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        await StorePublisherAsync<ICommandBus<TMessageId>>(typedCommand, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        await StorePublisherAsync<ICommandBus<TMessageId>>(command, cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }
}
