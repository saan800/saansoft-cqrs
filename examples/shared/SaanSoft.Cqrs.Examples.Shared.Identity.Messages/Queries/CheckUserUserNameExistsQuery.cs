namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Queries;

public class CheckUserUserNameExistsQuery : Query<CheckUserUserNameExistsQuery, Guid?>
{
    public CheckUserUserNameExistsQuery(string userName, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        UserName = userName;
    }

    public CheckUserUserNameExistsQuery(string userName, IMessage triggeredByMessage) : base(triggeredByMessage)
    {
        UserName = userName;
    }

    public string UserName { get; set; }
}

