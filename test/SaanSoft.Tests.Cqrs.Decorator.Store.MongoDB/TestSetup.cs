using EphemeralMongo;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB;

public class TestSetup : SaanSoft.Tests.Cqrs.Common.TestSetup
{
    protected TestSetup() : base()
    {
        string name = Guid.NewGuid().ToString("N");
        Lazy<IMongoClient> mongoClient = new(() => new MongoClient(_temporaryMongoDb.Value.ConnectionString));
        _database = new Lazy<IMongoDatabase>(() => mongoClient.Value.GetDatabase(name));
        MongoDbConfiguration.Setup();
    }

    private readonly Lazy<IMongoRunner> _temporaryMongoDb = new Lazy<IMongoRunner>((Func<IMongoRunner>)(() => MongoRunner.Run(new MongoRunnerOptions
    {
        StandardOuputLogger = (Logger)(_ => { }),
        AdditionalArguments = "--nojournal"
    })));
    private readonly Lazy<IMongoDatabase> _database;
    protected IMongoDatabase Database => _database.Value;
}
