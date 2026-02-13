namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Base interface with common properties for RouterOptions
/// You should never directly inherit from IBaseMessageRouterOptions.
/// Use <see cref="IDefaultExternalMessageRouterOptions"/> or <see cref="IExternalMessageProvider"/> instead
/// </summary>
public interface IBaseMessageRouterOptions
{
    /// <summary>
    /// Timeout for handling a message, especially those messages where the publisher is waiting for a response.
    /// If a message implements <see cref="ITimeout"/>, that value will overwrite the default at runtime.
    /// </summary>
    TimeSpan Timeout { get; set; }
}

/// <summary>
/// Provide options for use with local message transport.
/// </summary>
public interface ILocalMessageRouterOptions : IBaseMessageRouterOptions;

/// <summary>
/// Provide the default router options for use with external message transports.
/// At run time the defaults will be cloned for each message context.
/// </summary>
public interface IDefaultExternalMessageRouterOptions : IBaseMessageRouterOptions
{
    /// <summary>
    /// Create a IExternalMessageProviderOptions from the defaults
    /// </summary>
    IExternalMessageProviderOptions Clone(bool waitForExecution, bool expectSingleHandler);
}

/// <summary>
/// Transport options provided to the IExternalMessageProviderOptions.
/// </summary>
public interface IExternalMessageProviderOptions : IBaseMessageRouterOptions
{
    /// <summary>
    /// Whether to wait for the external handler to deal with the message, or if its fire-and-forget.
    /// Configured at run time for each message, depending on the message type and IMessageBus method used.
    /// </summary>
    /// <remarks>
    /// If WaitForExecution=false, the IExternalMessageProviderOptions should still return a ExternalResponse,
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
