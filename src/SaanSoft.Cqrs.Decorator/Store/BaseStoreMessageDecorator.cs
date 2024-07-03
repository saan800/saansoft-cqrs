namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageDecorator<TMessageId, TMessage>(
    IMessageRepository<TMessageId, TMessage> repository) :
    IMessageBusDecorator
    where TMessage : IMessage<TMessageId>
    where TMessageId : struct
{
    protected async Task StoreMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        if (!message.IsReplay)
        {
            await repository.InsertAsync(message, cancellationToken);
        }
    }
}
