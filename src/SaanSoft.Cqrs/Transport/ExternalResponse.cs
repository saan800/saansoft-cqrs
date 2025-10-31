namespace SaanSoft.Cqrs.Transport;

public sealed class ExternalResponse
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
    /// TResponse for queries/commands with responses
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
    /// Create a successful ExternalResponse without a payload.
    /// ie. use with <see cref="ICommand"/> or <see cref="IEvent{TEntityKey}"/> which don't have responses.
    /// </summary>
    public static ExternalResponse FromSuccess()
        => FromSuccess(null);

    /// <summary>
    /// Create a successful ExternalResponse with a payload that will be returned to the publisher.
    /// ie. use with <see cref="ICommand{TResponse}"/> and <see cref="IQuery{TResponse}"/>
    /// </summary>
    public static ExternalResponse FromSuccess(object? payload)
        => new() { Success = true, Payload = payload };

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
