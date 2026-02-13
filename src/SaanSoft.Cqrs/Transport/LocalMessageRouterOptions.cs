namespace SaanSoft.Cqrs.Transport;

public class LocalMessageRouterOptions : ILocalMessageRouterOptions
{
    /// <summary>
    /// Timeout for handling a message, especially those messages where the publisher is waiting for a response.
    /// If a message implements <see cref="ITimeout"/>, that value will overwrite the default at runtime.
    /// Defaults to 15min.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);
}
