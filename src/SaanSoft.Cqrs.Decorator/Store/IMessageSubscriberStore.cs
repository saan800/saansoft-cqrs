using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IMessageSubscriberStore<TMessageId, in TMessage>
    where TMessageId : struct
    where TMessage : IMessage<TMessageId>
{
    /// <summary>
    /// Store the class of the subscriber of a message, and whether the message succeeded or failed.
    /// Used with <see ref="Store[Command|Event|Query]SubscriberDecorator"/>
    /// </summary>
    /// <param name="message">Message being handled / subscribed to</param>
    /// <param name="subscriberType">Class that subscribes the message</param>>
    /// <param name="exception">
    /// Exception thrown when the message was handled by the subscriber.
    /// Providing this will indicate that the message failed in the subscriber.
    /// If exception is null, then indicates the  message succeeded in the subscriber.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertSubscriberAsync(IMessage<TMessageId> message, Type subscriberType, Exception? exception = null, CancellationToken cancellationToken = default);
}
