namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Record of a handler processing a message and the resulting status
/// </summary>
public sealed record HandlerRecord(
    string HandlerName,
    HandlerStatus Status,
    DateTime? HandledOnUtc = null,
    string? ErrorMessage = null,
    Exception? Exception = null
);
