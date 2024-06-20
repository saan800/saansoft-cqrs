namespace SaanSoft.Cqrs.Decorator.Store.Models;

public class MessageSubscriberRecord<TId> where TId : struct
{
    public required TId Id { get; set; }

    public required string MessageTypeName { get; set; }

    public required string SubscriberTypeName { get; set; }

    public required DateTime CreatedOnUtc { get; set; }

    public DateTime LastMessageOnUtc { get; set; }
}
