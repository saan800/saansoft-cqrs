using System.Diagnostics;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessagePublisherDecorator(IMessagePublisherStore store)
{
    protected async Task StorePublisher<TMessage, TPublisher>(CancellationToken cancellationToken)
    {
        var messageType = typeof(TMessage);

        var callerClassType = new StackTrace().GetFrames()
            .Where(f => !string.IsNullOrWhiteSpace(f.GetMethod()?.DeclaringType?.Namespace))
            .Where(f => f.GetMethod()!.DeclaringType.IsClass)
            .Where(f => f.GetMethod()!.DeclaringType.IsVisible)
            .Where(f => !f.GetMethod()!.DeclaringType.Namespace.StartsWith("System"))
            .Where(f => !f.GetMethod()!.DeclaringType.Name.Equals(nameof(BaseStoreMessagePublisherDecorator)))
            .FirstOrDefault(f => f.GetMethod()!.DeclaringType.GetInterface(typeof(TPublisher).Name) == null)
            ?.GetMethod()
            ?.DeclaringType;

        var messageTypeName = messageType.FullName ?? messageType.Name;
        var publishedByName = callerClassType?.FullName ?? callerClassType?.Name ?? "Unknown";
        await store.UpsertPublisherAsync(messageTypeName, publishedByName, cancellationToken);
    }
}
