using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the command's metadata.
///
/// Should be used in conjunction with <see cref="StoreCommandDecorator{TMessageId}"/>
/// </summary>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreCommandPublisherDecorator<TMessageId>(IBaseCommandBus<TMessageId> next) :
      BaseStoreMessagePublisherDecorator<TMessageId>,
      IBaseCommandBus<TMessageId> where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        await StorePublisherAsync<IBaseCommandBus<TMessageId>>(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        await StorePublisherAsync<IBaseCommandBus<TMessageId>>(typedCommand, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        await StorePublisherAsync<IBaseCommandBus<TMessageId>>(command, cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }
}
