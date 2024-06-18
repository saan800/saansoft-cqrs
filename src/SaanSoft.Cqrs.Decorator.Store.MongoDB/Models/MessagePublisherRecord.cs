namespace SaanSoft.Cqrs.Decorator.Store.MongoDB.Models;

public class MessagePublisherRecord<TId> where TId : struct
{
    public required TId Id { get; set; }

    public required string MessageTypeName { get; set; }

    public required string PublisherTypeName { get; set; }

    public required DateTime CreatedOnUtc { get; set; }

    public DateTime LastMessageOnUtc { get; set; }
}
