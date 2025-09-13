namespace SaanSoft.Cqrs.Core.Transport;

/// <summary>
/// Base interface with common properties for ExternalTransportOptions
/// You should never directly inherit from IBaseExternalTransportOptions
/// Use <see cref="IDefaultExternalTransportOptions"/> or <see cref="IExternalTransportOptions"/> instead
/// </summary>
public interface IBaseExternalTransportOptions
{
    /// <summary>
    /// Timeout for handling a message, especially those messages where the publisher is waiting for a result.
    /// If a message implements <see cref="ITimeout"/>, that value will be set here at runtime.
    /// Defaults to 15min.
    /// </summary>
    TimeSpan Timeout { get; set; }
}

/// <summary>
/// Provide the default transport options for use with external message transports.
/// At run time the defaults will be cloned for each message.
/// </summary>
public interface IDefaultExternalTransportOptions : IBaseExternalTransportOptions
{
    /// <summary>
    /// Create a IExternalTransportOptions from the defaults
    /// </summary>
    IExternalTransportOptions Clone(bool waitForExecution);
}

/// <summary>
/// Transport options provided to the IExternalMessageTransport.
/// Also used in TransportContext and IExternalTransportMiddleware to be enriched before publishing
/// </summary>
public interface IExternalTransportOptions : IBaseExternalTransportOptions
{
    /// <summary>
    /// Whether to wait for the external handler to deal with the message, or if its fire-and-forget.
    /// Configured at run time for each message, depending on the message type and IMessageBus method used.
    /// </summary>
    /// <remarks>
    /// If WaitForExecution=false, the IExternalMessageTransport should still return a ExternalResult,
    /// but Success is that the message was published successfully, rather than handled successfully.
    /// </remarks>
    bool WaitForExecution { get; set; }

    /// <summary>
    /// Whether we are expecting a single handler for the message (ie ICommands and IQueries) or multiple handlers
    /// (ie IEvents).
    /// </summary>
    bool ExpectSingleHandler { get; set; }
}
