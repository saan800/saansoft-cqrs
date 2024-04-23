using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Cqrs.Messages;

public sealed class CommandResponse : IMessageResponse
{
    [SetsRequiredMembers]
    public CommandResponse()
    {
    }

    [SetsRequiredMembers]
    public CommandResponse(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Was the command successful or not
    /// </summary>
    public required bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Reason for failure, if applicable
    /// </summary>
    public string? ErrorMessage { get; set; }
}
