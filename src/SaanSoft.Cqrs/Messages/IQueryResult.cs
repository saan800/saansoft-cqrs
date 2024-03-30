namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never inherit from IQueryResult directly
/// use <see cref="IQueryResult{TPayload}"/> instead
/// </summary>
public interface IQueryResult
{
    bool IsSuccess { get; set; }
    string? ErrorMessage { get; set; }
}

public interface IQueryResult<TPayload> : IQueryResult
{
    TPayload? Payload { get; set; }
}
