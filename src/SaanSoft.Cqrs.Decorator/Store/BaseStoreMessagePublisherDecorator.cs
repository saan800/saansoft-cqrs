using System.Diagnostics;
using SaanSoft.Cqrs.Decorator.Store.Utilities;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessagePublisherDecorator<TMessageId> :
    IBaseBus
    where TMessageId : struct
{
    protected Task StorePublisherAsync<TMessageBus>(IBaseMessage<TMessageId> message, CancellationToken cancellationToken)
    {
        var callerClassType = new StackTrace().GetFrames()
            .Where(f => !string.IsNullOrWhiteSpace(f.GetMethod()?.DeclaringType?.Namespace))
            .Where(f => f.GetMethod()!.DeclaringType.IsClass)
            .Where(f => f.GetMethod()!.DeclaringType.IsVisible)
            .Where(f => !f.GetMethod()!.DeclaringType.Namespace.StartsWith("System"))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseBus)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseCommandBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseCommandSubscriptionBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseEventBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseEventSubscriptionBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseQueryBus<>)))
            .Where(f => !f.GetMethod().DeclaringType.IsAssignableTo(typeof(IBaseQuerySubscriptionBus<>)))
            .FirstOrDefault(f => !f.GetMethod()!.DeclaringType.IsAssignableTo(typeof(TMessageBus)))
            ?.GetMethod()
            ?.DeclaringType;

        message.Metadata.AddPublisher(callerClassType);
        return Task.CompletedTask;
    }
}
