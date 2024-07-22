namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class QueryHandlerRepositoryTests : BaseHandlerRepositoryTests<IQuery, QueryRepository>
{
    public QueryHandlerRepositoryTests()
    {
        SutRepository = new QueryRepository(Database, Logger);
    }

    protected override MyQuery CreateNewMessage() => new MyQuery();
}
