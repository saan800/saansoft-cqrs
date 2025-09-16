namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Base interface with common properties for ProcessorOptions
/// You should never directly inherit from IBaseProcessorOptions.
/// Use <see cref="IDefaultProcessorOptions"/> or <see cref="IExternalProcessorOptions"/> instead
/// </summary>
public interface IBaseProcessorOptions
{
    /// <summary>
    /// Timeout for handling a message, especially those messages where the publisher is waiting for a result.
    /// If a message implements <see cref="ITimeout"/>, that value will overwrite the default at runtime.
    /// </summary>
    TimeSpan Timeout { get; set; }
}

/// <summary>
/// Provide the default processor options for use with in memory and external message transports.
/// At run time the defaults will be cloned for each message.
/// </summary>
public interface IDefaultProcessorOptions : IBaseProcessorOptions
{
    /// <summary>
    /// Create a IExternalProcessorOptions from the defaults
    /// </summary>
    IExternalProcessorOptions Clone(bool waitForExecution, bool expectSingleHandler);
}

/// <summary>
/// Transport options provided to the IExternalMessageTransport.
/// Also used in TransportContext and IExternalTransportMiddleware to be enriched before publishing
/// </summary>
public interface IExternalProcessorOptions : IBaseProcessorOptions
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
    /// <remarks>
    /// Some message transports can differ if setting up messaging as 1-to-many (ie pub/sub) vs 1-to-1
    /// </remarks>
    bool ExpectSingleHandler { get; set; }
}
