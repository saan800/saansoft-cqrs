namespace SaanSoft.Cqrs.Decorator.Store;

public interface IMessageSubscriberStore
{
    /// <summary>
    /// Store the class name of the subscriber(s) of messages.
    /// Used with <see ref="Store[Command|Event|Query]SubscriberDecorator"/>
    /// </summary>
    /// <param name="messageTypeName">Message - type's full name</param>
    /// <param name="subscriberClassTypeNames">Class that subscribes to the message - type's full name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertSubscriberAsync(string messageTypeName, IEnumerable<string> subscriberClassTypeNames, CancellationToken cancellationToken = default);
}
