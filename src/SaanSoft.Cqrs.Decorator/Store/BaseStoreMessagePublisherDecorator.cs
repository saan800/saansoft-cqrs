using System.Diagnostics;
using SaanSoft.Cqrs.Messages;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessagePublisherDecorator<TMessageId, TMessage>(IMessagePublisherStore<TMessageId, TMessage> store) :
    IPublisherDecorator
    where TMessage : IMessage<TMessageId>
    where TMessageId : struct
{
    protected async Task StorePublisher<TPublisher>(IMessage<TMessageId> message, CancellationToken cancellationToken)
    {
        var callerClassType = new StackTrace().GetFrames()
            .Where(f => !string.IsNullOrWhiteSpace(f.GetMethod()?.DeclaringType?.Namespace))
            .Where(f => f.GetMethod()!.DeclaringType.IsClass)
            .Where(f => f.GetMethod()!.DeclaringType.IsVisible)
            .Where(f => !f.GetMethod()!.DeclaringType.Namespace.StartsWith("System"))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IDecorator)))
            .FirstOrDefault(f => !f.GetMethod()!.DeclaringType.IsAssignableTo(typeof(TPublisher)))
            ?.GetMethod()
            ?.DeclaringType;

        if (callerClassType != null)
        {
            await store.UpsertPublisherAsync(message, callerClassType, cancellationToken);
        }
    }
}
