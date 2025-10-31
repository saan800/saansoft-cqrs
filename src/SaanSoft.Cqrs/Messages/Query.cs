namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TResponse> : MessageBase, IQuery<TResponse>
{
    public Query() : base() { }

    /// <inheritdoc/>
    protected Query(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
