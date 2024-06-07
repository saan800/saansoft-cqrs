namespace SaanSoft.Cqrs.Store;

public interface IMessagePublisherStore
{
    /// <summary>
    /// Store the class name of the publishers of messages.
    /// Used with <see ref="StoreCommandPublisherDecorator"/>
    /// </summary>
    /// <param name="messageTypeName">Message - type's full name</param>
    /// <param name="publisherClassTypeName">Class that published the message - type's full name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertPublisherAsync(string messageTypeName, string publisherClassTypeName, CancellationToken cancellationToken = default);
}
