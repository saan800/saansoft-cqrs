namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class QueryHandlerRepositoryTests : BaseHandlerRepositoryTests<MyQuery, QueryRepository, Query>
{
    public QueryHandlerRepositoryTests()
    {
        SutRepository = new QueryRepository(Database, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyQuery CreateNewMessage() => new MyQuery();
}
