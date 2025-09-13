namespace SaanSoft.Cqrs.Transport;

public class ExternalProcessorOptions : IExternalProcessorOptions
{
    public virtual TimeSpan Timeout { get; set; }

    public bool WaitForExecution { get; set; }

    public bool ExpectSingleHandler { get; set; }
}
