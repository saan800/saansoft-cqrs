namespace SaanSoft.Cqrs.Handler;

public abstract class MessageBase : IMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? CorrelationId { get; set; }
    public string? AuthenticationId { get; set; }
    public DateTime OccurredOn { get; set; } = default;
    public string? TriggeredByMessageId { get; set; }
    public bool IsReplay { get; set; }
}
