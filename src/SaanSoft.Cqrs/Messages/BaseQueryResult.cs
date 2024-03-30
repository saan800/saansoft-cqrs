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

    public required bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }
}
