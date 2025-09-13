namespace SaanSoft.Cqrs.Transport;

public sealed class ExternalResult
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
    /// TResult for queries/commands with results
    /// </summary>
    public object? Payload { get; init; }

    /// <summary>
    /// Error message if the operation was not successful.
    /// </summary>
    /// <remarks>
    /// If a operation failed, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Exception that occurred if the operation was not successful
    /// </summary>
    /// <remarks>
    /// If a operation failed, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Create a successful ExternalResult without a payload.
    /// ie. use with <see cref="ICommand"/> or <see cref="IEvent{TEntityKey}"/> which don't have results.
    /// </summary>
    public static ExternalResult FromSuccess()
        => FromSuccess(null);

    /// <summary>
    /// Create a successful ExternalResult with a resulting payload that will be returned to the publisher.
    /// ie. use with <see cref="ICommand{TResult}"/> and <see cref="IQuery{TResult}"/>
    /// </summary>
    public static ExternalResult FromSuccess(object? payload)
        => new() { Success = true, Payload = payload };

    /// <summary>
    /// Create a failed ExternalResult with an errorMessage
    /// </summary>
    public static ExternalResult FromError(string errorMessage) => FromError(errorMessage, null);

    /// <summary>
    /// Create a failed ExternalResult with an exception
    /// </summary>
    public static ExternalResult FromException(Exception exception) => FromError(null, exception);

    /// <summary>
    /// Create a failed ExternalResult
    /// </summary>
    /// <remarks>
    /// If a operation is not successful, then should provide at least one of
    /// <see cref="ErrorMessage"/> or <see cref="Exception"/>
    /// </remarks>
    public static ExternalResult FromError(string? errorMessage, Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(errorMessage) && exception != null)
            errorMessage = exception.Message;

        return new ExternalResult
        {
            Success = false,
            ErrorMessage = errorMessage?.Trim(),
            Exception = exception
        };
    }
}
