namespace SaanSoft.Cqrs.Transport;

public class ExternalTransportOptions : IExternalTransportOptions
{
    public bool WaitForExecution { get; set; }

    public virtual TimeSpan? Timeout { get; set; }

    public virtual Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);
}

