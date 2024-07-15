namespace SaanSoft.Cqrs.Messages;

public interface IEntityEvent<TEntity>
{
    TEntity? ApplyEvent(TEntity? entity);
}
