namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageDecorator<TMessage>(
    IMessageRepository<TMessage> repository) :
    IMessageBusDecorator
    where TMessage : IMessage
{
    protected async Task StoreMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        if (!message.IsReplay)
        {
            await repository.InsertAsync(message, cancellationToken);
        }
    }
}
