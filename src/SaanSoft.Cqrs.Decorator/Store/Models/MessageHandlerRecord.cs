namespace SaanSoft.Cqrs.Decorator.Store.Models;

public class MessageHandlerRecord<TId> where TId : struct
{
    public required TId Id { get; set; }

    public required string MessageTypeName { get; set; }

    public required string MessageAssembly { get; set; }

    public required string HandlerTypeName { get; set; }

    public required string HandlerAssembly { get; set; }

    public required DateTime CreatedOnUtc { get; set; }

    public required DateTime LastMessageOnUtc { get; set; }

    public required TId LastMessageId { get; set; }

    public TId? LastCompletedMessageId { get; set; }

    public List<FailedMessage> LastFailedMessages { get; set; } = [];

    public class FailedMessage
    {
        public required TId MessageId { get; set; }
        public required DateTime MessageOnUtc { get; set; }
        public required LogException Exception { get; set; }
    }

    public class LogException
    {
        public required string Message { get; set; }
        public required string TypeName { get; set; }
        public string? StackTrace { get; set; }
    }
}
