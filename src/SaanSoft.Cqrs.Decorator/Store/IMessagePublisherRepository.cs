namespace SaanSoft.Cqrs.Decorator.Store;

public interface IMessagePublisherRepository<TMessageId, in TMessage>
    where TMessageId : struct
    where TMessage : IMessage<TMessageId>
{
    /// <summary>
    /// Store the class of the publishers of messages.
    /// Used with <see ref="Store[Command|Event|Query]PublisherDecorator"/>
    /// </summary>
    /// <param name="message">Message being published</param>
    /// <param name="publisherType">Class that published the message</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpsertPublisherAsync(IMessage<TMessageId> message, Type publisherType, CancellationToken cancellationToken = default);
}
