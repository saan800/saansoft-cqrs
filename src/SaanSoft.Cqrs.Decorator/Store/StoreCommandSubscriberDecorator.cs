using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandSubscriberDecorator(IServiceProvider serviceProvider, ICommandSubscriberStore store, ICommandSubscriber<Guid> next)
    : StoreCommandSubscriberDecorator<Guid>(serviceProvider, store, next);

public abstract class StoreCommandSubscriberDecorator<TMessageId>(IServiceProvider serviceProvider, ICommandSubscriberStore store, ICommandSubscriber<TMessageId> next) :
    BaseStoreMessageSubscriberDecorator(serviceProvider, store),
    ICommandSubscriber<TMessageId> where TMessageId : struct
{
    protected override bool AllowMultipleSubscribers => false;

    public async Task<CommandResponse> RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TMessageId>
    {
        if (!command.IsReplay)
        {
            await StoreSubscriber<TCommand, ICommandHandler<TCommand>>(cancellationToken);
        }
        return await next.RunAsync(command, cancellationToken);
    }
}
