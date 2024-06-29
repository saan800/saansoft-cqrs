namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IMongoDbRepository
{
    string CollectionName { get; }

    Task EnsureCollectionIndexes(CancellationToken cancellationToken = default);
}
