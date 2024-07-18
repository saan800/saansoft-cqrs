using SaanSoft.Cqrs.Decorator.Store.Models;

namespace SaanSoft.Cqrs.Decorator.Store.Utilities;

public static class MetadataExtensions
{
    public static void AddPublisher(this MessageMetadata metadata, Type? publisherType)
    {
        if (publisherType == null) return;

        metadata.Add(StoreConstants.PublisherKey, publisherType.GetTypeFullName());
        metadata.Add(StoreConstants.PublisherAssemblyKey, publisherType.GetAssemblyName());
    }

    public static void AddHandlerMetadata(this MessageMetadata metadata, MessageHandler handler)
    {
        var handlers = metadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey)
            ?? [];

        if (handler.Succeeded)
        {
            // if handler succeeded, make sure to remove any previous error states for that handler
            handlers = handlers
                .Where(x => !x.TypeFullName.Equals(handler.TypeFullName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        handlers.Add(handler);

        metadata.Remove(StoreConstants.HandlersKey);
        metadata.Add(StoreConstants.HandlersKey, handlers);
    }

    public static MessageHandler BuildMessageHandler(this Type handlerType, Exception? exception = null)
        => new()
        {
            TypeFullName = handlerType.GetTypeFullName(),
            Assembly = handlerType.GetAssemblyName(),
            HandledOnUtc = DateTime.UtcNow,
            Succeeded = exception == null,
            Exception = exception == null
                ? null
                : new MessageHandler.LogException
                {
                    TypeFullName = exception.GetType().GetTypeFullName(),
                    Message = exception.Message
                }
        };
}
