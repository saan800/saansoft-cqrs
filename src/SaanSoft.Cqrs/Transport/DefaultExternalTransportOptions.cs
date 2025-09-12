namespace SaanSoft.Cqrs.Transport;

public class DefaultExternalTransportOptions : IDefaultExternalTransportOptions
{
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);

    public IExternalTransportOptions Clone(bool waitForExecution)
    {
        var eto = new ExternalTransportOptions
        {
            WaitForExecution = waitForExecution,
            Timeout = Timeout,
        };

        return eto;
    }
}

