namespace SaanSoft.Cqrs.Transport;

public class DefaultProcessorOptions : IDefaultProcessorOptions
{
    public DefaultProcessorOptions()
    {
    }

    public DefaultProcessorOptions(TimeSpan timeout)
    {
        Timeout = timeout;
    }

    /// <summary>
    /// Timeout for handling a message, especially those messages where the publisher is waiting for a result.
    /// If a message implements <see cref="ITimeout"/>, that value will overwrite the default at runtime.
    /// Defaults to 15min.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);

    public virtual IExternalProcessorOptions Clone(bool waitForExecution)
    {
        var eto = new ExternalProcessorOptions
        {
            WaitForExecution = waitForExecution,
            Timeout = Timeout,
        };

        return eto;
    }
}

