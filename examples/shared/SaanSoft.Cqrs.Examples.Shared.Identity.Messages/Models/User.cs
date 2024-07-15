namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Models;

public class User
{
    public Guid Key { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Biography { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public string CreatedBy { get; set; }

    public DateTime LastUpdatedOnUtc { get; set; }
}
