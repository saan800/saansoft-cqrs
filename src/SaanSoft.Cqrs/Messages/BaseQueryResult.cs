namespace SaanSoft.Cqrs.Messages;

public abstract class BaseQueryResult : IQueryResult
{
    protected BaseQueryResult()
    {
        IsSuccess = true;
    }

    protected BaseQueryResult(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; set; } = false;

    public string? ErrorMessage { get; set; }
}
