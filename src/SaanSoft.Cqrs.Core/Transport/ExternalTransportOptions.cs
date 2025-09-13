namespace SaanSoft.Cqrs.Transport;

public class ExternalTransportOptions : IExternalTransportOptions
{
    public bool WaitForExecution { get; set; }

    public bool ExpectSingleHandler { get; set; }

    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);
}

