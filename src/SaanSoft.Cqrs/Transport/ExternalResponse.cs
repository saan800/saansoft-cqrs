namespace SaanSoft.Cqrs.Transport;

public class ExternalResponse
{
    /// <summary>
    /// If the operation was successful.
    /// If not successful, check <see cref="ErrorMessage"/> and <see cref="Exception"/>
    /// </summary>
    /// <remarks>
    /// If a operation failed, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public bool Success { get; init; }

    /// <summary>
    /// Error message if the operation was not successful.
    /// 
    /// ErrorMessage is generally populated by the message bus, provider or transport - not the
    /// message handler itself
    /// </summary>
    /// <remarks>
    /// If a operation failed, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Exception that occurred if the operation was not successful
    ///
    /// Usually populated from exceptions thrown in the handler
    /// </summary>
    /// <remarks>
    /// If a operation failed, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Create a successful ExternalResponse without a payload.
    /// ie. use with <see cref="ICommand"/> or <see cref="IEvent{TEntityKey}"/> which don't have responses.
    /// </summary>
    public static ExternalResponse FromSuccess()
        => new() { Success = true };

    /// <summary>
    /// Create a failed ExternalResponse with an errorMessage
    /// </summary>
    public static ExternalResponse FromError(string errorMessage) => FromError(errorMessage, null);

    /// <summary>
    /// Create a failed ExternalResponse with an exception
    /// </summary>
    public static ExternalResponse FromException(Exception exception) => FromError(null, exception);

    /// <summary>
    /// Create a failed ExternalResponse
    /// </summary>
    /// <remarks>
    /// If a operation is not successful, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public static ExternalResponse FromError(string? errorMessage, Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(errorMessage) && exception != null)
            errorMessage = exception.Message;

        return new ExternalResponse
        {
            Success = false,
            ErrorMessage = errorMessage?.Trim(),
            Exception = exception
        };
    }
}

public class ExternalResponse<TRespose> : ExternalResponse
{
    /// <summary>
    /// TResponse for queries/commands with responses
    /// </summary>
    public TRespose? Payload { get; init; }

    /// <summary>
    /// Create a successful ExternalResponse with a payload that will be returned to the publisher.
    /// ie. use with <see cref="ICommand{TResponse}"/> and <see cref="IQuery{TResponse}"/>
    /// </summary>
    public static ExternalResponse<TRespose> FromSuccess(TRespose? payload)
        => new() { Success = true, Payload = payload };

    /// <summary>
    /// Create a failed ExternalResponse with an errorMessage
    /// </summary>
    public static new ExternalResponse<TRespose> FromError(string errorMessage)
        => FromError(errorMessage, null);
    /// <summary>
    /// Create a failed ExternalResponse with an exception
    /// </summary>
    public static new ExternalResponse<TRespose> FromException(Exception exception)
        => FromError(null, exception);

    /// <summary>
    /// Create a failed ExternalResponse
    /// </summary>
    /// <remarks>
    /// If a operation is not successful, then should provide at least one of
    /// <see cref="ExternalResponse.ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public static new ExternalResponse<TRespose> FromError(string? errorMessage, Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(errorMessage) && exception != null)
            errorMessage = exception.Message;

        return new ExternalResponse<TRespose>
        {
            Success = false,
            ErrorMessage = errorMessage?.Trim(),
            Exception = exception
        };
    }
}
