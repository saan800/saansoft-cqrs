namespace SaanSoft.Cqrs.Transport;

public class DefaultExternalTransportOptions : IDefaultExternalTransportOptions
{
    public virtual TimeSpan? Timeout { get; set; }

    public virtual Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

    public IExternalTransportOptions Clone(bool waitForExecution)
    {
        var eto = new ExternalTransportOptions
        {
            WaitForExecution = waitForExecution,
            Timeout = Timeout,
        };

        foreach (var h in Headers)
        {
            eto.Headers.Add(h.Key, h.Value);
        }

        return eto;
    }
}

