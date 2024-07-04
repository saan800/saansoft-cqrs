namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class QueryHandlerRepositoryTests : BaseHandlerRepositoryTests<MyQuery, QueryRepository, Query<Guid>>
{
    public QueryHandlerRepositoryTests()
    {
        SutRepository = new QueryRepository(Database, IdGenerator, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyQuery CreateNewMessage() => new MyQuery();
}
