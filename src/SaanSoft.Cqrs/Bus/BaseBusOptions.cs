using Microsoft.Extensions.Logging;

namespace SaanSoft.Cqrs.Bus;

/// <summary>
/// Common options for command/event/query buses
/// Shouldn't be used directly
/// </summary>
public abstract class BaseBusOptions
{
    /// <summary>
    /// The log level for log non-critical messages
    /// </summary>
    /// <value>@default Debug</value>
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;
}
