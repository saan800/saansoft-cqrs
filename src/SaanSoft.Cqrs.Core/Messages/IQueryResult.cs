namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// You should never inherit from IQueryResult directly
/// use <see cref="IQueryResult{TPayload}"/> instead
/// </summary>
public interface IQueryResult
{
    bool IsError { get; set; }
    string? ErrorMessage { get; set; }
}

public interface IQueryResult<TPayload> : IQueryResult
{
    TPayload? Payload { get; set; }
}
