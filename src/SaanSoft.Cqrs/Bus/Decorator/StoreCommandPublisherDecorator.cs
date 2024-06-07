using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store;

namespace SaanSoft.Cqrs.Bus.Decorator;

public class StoreCommandPublisherDecorator(ICommandPublisherStore store, ICommandPublisher<Guid> next)
    : StoreCommandPublisherDecorator<Guid>(store, next);

public abstract class StoreCommandPublisherDecorator<TMessageId>(ICommandPublisherStore store, ICommandPublisher<TMessageId> next) :
      BaseMessagePublisherDecorator(store),
      ICommandPublisher<TMessageId> where TMessageId : struct
{
    public async Task<CommandResponse> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<TCommand, ICommandPublisher<TMessageId>>(cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<TCommand, ICommandPublisher<TMessageId>>(cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }
}
