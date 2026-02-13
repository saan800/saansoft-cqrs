namespace SaanSoft.Cqrs.Transport;

public class ExternalMessageProviderOptions : IExternalMessageProviderOptions
{
    public virtual TimeSpan Timeout { get; set; }

    public bool WaitForExecution { get; set; }

    public bool ExpectSingleHandler { get; set; }
}
