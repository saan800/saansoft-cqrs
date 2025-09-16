namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TResult> : MessageBase, IQuery<TResult>
{
    public Query() : base() { }

    /// <inheritdoc/>
    protected Query(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
