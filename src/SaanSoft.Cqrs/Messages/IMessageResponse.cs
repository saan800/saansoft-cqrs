namespace SaanSoft.Cqrs.Messages;

public interface IMessageResponse
{
    /// <summary>
    /// Was the message successful or not
    /// </summary>
    bool IsSuccess { get; set; }

    /// <summary>
    /// Reason for failure, if applicable
    /// </summary>
    string? ErrorMessage { get; set; }
}
