namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Models;

public class UserSummary
{
    public Guid Key { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
