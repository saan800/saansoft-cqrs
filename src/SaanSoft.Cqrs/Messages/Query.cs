namespace SaanSoft.Cqrs.Messages;

/// <inheritdoc cref="IQuery{TResponse}"/>
public abstract class Query<TResponse> : MessageBase, IQuery<TResponse>
{
    public Query() : base() { }

    /// <inheritdoc/>
    protected Query(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
