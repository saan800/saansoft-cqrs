using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Cqrs.Core.Messages;

public sealed class CommandResult
{
    [SetsRequiredMembers]
    public CommandResult()
    {
        IsError = false;
    }

    [SetsRequiredMembers]
    public CommandResult(string errorMessage)
    {
        IsError = true;
        ErrorMessage = errorMessage;
    }

    public required bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
}
