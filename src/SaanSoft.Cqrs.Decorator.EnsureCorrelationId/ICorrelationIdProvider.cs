namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

public interface ICorrelationIdProvider
{
    string? Get();
}
