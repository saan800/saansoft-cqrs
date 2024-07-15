namespace SaanSoft.Cqrs.Common.Messages;

public interface IEntityEvent<TEntity>
{
    TEntity? ApplyEvent(TEntity? entity);
}
