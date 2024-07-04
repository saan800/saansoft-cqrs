using System.Diagnostics;
using SaanSoft.Cqrs.Decorator.Store.Utilities;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessagePublisherDecorator<TMessageId> :
    IMessageBusDecorator
    where TMessageId : struct
{
    protected async Task StorePublisherAsync<TMessageBus>(IMessage<TMessageId> message, CancellationToken cancellationToken)
    {
        var callerClassType = new StackTrace().GetFrames()
            .Where(f => !string.IsNullOrWhiteSpace(f.GetMethod()?.DeclaringType?.Namespace))
            .Where(f => f.GetMethod()!.DeclaringType.IsClass)
            .Where(f => f.GetMethod()!.DeclaringType.IsVisible)
            .Where(f => !f.GetMethod()!.DeclaringType.Namespace.StartsWith("System"))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IDecorator)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(ICommandBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(ICommandSubscriptionBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IEventBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IEventSubscriptionBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IQueryBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IQuerySubscriptionBus<>)))
            .FirstOrDefault(f => !f.GetMethod()!.DeclaringType.IsAssignableTo(typeof(TMessageBus)))
            ?.GetMethod()
            ?.DeclaringType;

        message.Metadata.AddPublisher(callerClassType);
    }
}
