namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Optional interface to add to messages, so can set a timeout on handling it.
///
/// Especially useful when used in conjunction with external message transports to ensure
/// processing of messages have enough time to run, and also that they don't run too long and hang.
/// </summary>
public interface ITimeout
{
    /// <summary>
    /// When to timeout handling of messages
    /// </summary>
    TimeSpan Timeout { get; }
}
