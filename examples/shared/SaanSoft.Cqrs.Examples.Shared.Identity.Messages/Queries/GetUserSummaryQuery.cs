namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Queries;

public class GetUserSummaryQuery : Query<GetUserSummaryQuery, UserSummary?>
{
    public GetUserSummaryQuery(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        UserKey = userKey;
    }

    public GetUserSummaryQuery(Guid userKey, IMessage triggeredByMessage) : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public Guid UserKey { get; set; }
}
