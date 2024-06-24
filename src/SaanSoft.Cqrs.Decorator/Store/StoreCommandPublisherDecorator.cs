using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandPublisherDecorator(ICommandPublisherStore<Guid> store, ICommandPublisher<Guid> next)
    : StoreCommandPublisherDecorator<Guid>(store, next);

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreCommandPublisherDecorator<TMessageId>(ICommandPublisherStore<TMessageId> store, ICommandPublisher<TMessageId> next) :
      BaseStoreMessagePublisherDecorator<TMessageId, ICommand<TMessageId>>(store),
      ICommandPublisher<TMessageId> where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<ICommandPublisher<TMessageId>>(command, cancellationToken);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        await StorePublisher<ICommandPublisher<TMessageId>>(typedCommand, cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<ICommandPublisher<TMessageId>>(command, cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }
}
