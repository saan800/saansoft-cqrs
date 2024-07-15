namespace SaanSoft.Cqrs.Common.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseMessage{TMessageId}"/> instead
/// </summary>
public interface IBaseMessage
{
    /// <summary>
    /// When the command/event/query was raised.
    ///
    /// When running events in order, use MessageOnUtc to run them in the correct order
    /// </summary>
    DateTime MessageOnUtc { get; set; }

    /// <summary>
    /// Whether the message is being replayed or not
    ///
    /// Note:
    /// - Events should replay
    /// - Queries should replay
    /// - Commands should NOT replay
    /// </summary>
    bool IsReplay { get; set; }

    /// <summary>
    /// Metadata about:
    /// - the message itself
    /// - the user that triggered the message
    /// - correlation Id
    /// - if this message was triggered by another message
    /// - publishing class
    /// - handlers
    /// - etc...
    /// </summary>
    MessageMetadata Metadata { get; set; }
}

/// <summary>
/// Base interface with common properties for all messages
/// You should never directly inherit from IMessage
/// Use <see cref="IBaseEvent{TMessageId,TEntityKey}"/>, <see cref="IBaseQuery{TMessageId,TQuery,TResponse}"/> or <see cref="IBaseCommand{TMessageId}"/> instead
/// </summary>
public interface IBaseMessage<TMessageId> : IBaseMessage where TMessageId : struct
{
    /// <summary>
    /// Unique Id for the command/event/query
    /// This will normally be the EventStore (or CommandStore and QueryStore if using) primary key
    /// Also used to populate the Metadata.TriggeredById property of any subsequent messages it raises
    /// </summary>
    TMessageId Id { get; set; }
}