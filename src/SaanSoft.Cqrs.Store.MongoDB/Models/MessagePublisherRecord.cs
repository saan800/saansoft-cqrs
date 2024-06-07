namespace SaanSoft.Cqrs.Store.MongoDB.Models;

public class MessagePublisherRecord<TId> where TId : struct
{
    public required TId Id { get; set; }

    public required string MessageTypeName { get; set; }

    public required string PublisherTypeName { get; set; }

    public required DateTime CreatedOn { get; set; }

    public DateTime LastMessageOn { get; set; }
}
