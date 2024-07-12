namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Queries;

public class GetUserQuery : Query<GetUserQuery, User>
{
    public GetUserQuery(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        UserKey = userKey;
    }

    public GetUserQuery(Guid userKey, IMessage triggeredByMessage) : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public Guid UserKey { get; set; }
}
