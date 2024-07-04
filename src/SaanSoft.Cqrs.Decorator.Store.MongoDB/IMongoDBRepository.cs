namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IMongoDbRepository
{
    string CollectionName { get; }

    Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default);
}
