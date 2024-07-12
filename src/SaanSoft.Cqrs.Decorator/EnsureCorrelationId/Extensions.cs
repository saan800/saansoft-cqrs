namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

internal static class Extensions
{
    internal static string EnsureCorrelationId(this IEnumerable<ICorrelationIdProvider> providers, string? correlationId)
    {
        if (!string.IsNullOrWhiteSpace(correlationId)) return correlationId;

        foreach (var provider in providers)
        {
            correlationId = provider.Get();
            if (!string.IsNullOrWhiteSpace(correlationId)) return correlationId;
        }

        return Guid.NewGuid().ToString();
    }
}
