using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Cqrs.Messages;

public sealed class CommandResult
{
    [SetsRequiredMembers]
    public CommandResult()
    {
    }

    [SetsRequiredMembers]
    public CommandResult(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public required bool IsSuccess { get; set; } = true;

    public string? ErrorMessage { get; set; }
}
