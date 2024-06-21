using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandSubscriberDecorator(ICommandSubscriberStore<Guid> store, ICommandSubscriber<Guid> next)
    : StoreCommandSubscriberDecorator<Guid>(store, next);

public abstract class StoreCommandSubscriberDecorator<TMessageId>(ICommandSubscriberStore<TMessageId> store, ICommandSubscriber<TMessageId> next)
    : BaseStoreMessageSubscriberDecorator<TMessageId, ICommand<TMessageId>>(store),
      ICommandSubscriber<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        try
        {
            await next.RunAsync(command, cancellationToken);
            await Store.UpsertSubscriberAsync(command, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Store.UpsertSubscriberAsync(command, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public ICommandHandler<TCommand> GetHandler<TCommand>() where TCommand : ICommand<TMessageId>
        => next.GetHandler<TCommand>();
}