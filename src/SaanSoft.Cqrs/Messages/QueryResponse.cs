namespace SaanSoft.Cqrs.Messages;

public abstract class QueryResponse : IQueryResponse
{
    protected QueryResponse()
    {
        IsSuccess = true;
    }

    protected QueryResponse(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
