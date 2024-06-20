namespace SaanSoft.Cqrs.Decorator.Store.Models;

public class MessagePublisherRecord<TId> where TId : struct
{
    public required TId Id { get; set; }

    public required string MessageTypeName { get; set; }

    public required string MessageAssembly { get; set; }

    public required string PublisherTypeName { get; set; }

    public required string PublisherAssembly { get; set; }

    public required DateTime CreatedOnUtc { get; set; }

    public required DateTime LastMessageOnUtc { get; set; }

    public required TId LastMessageId { get; set; }
}
