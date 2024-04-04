namespace SaanSoft.Cqrs.Messages;

public abstract class BaseQueryResponse : IQueryResponse
{
    protected BaseQueryResponse()
    {
        IsSuccess = true;
    }

    protected BaseQueryResponse(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
