using System.Reflection;
using EphemeralMongo;
using MongoDB.Bson.Serialization;
using SaanSoft.Cqrs.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Store.MongoDB;

public class TestSetup : SaanSoft.Tests.Cqrs.Common.TestSetup
{
    protected TestSetup() : base()
    {
        string name = Guid.NewGuid().ToString("N");
        Lazy<IMongoClient> mongoClient = new Lazy<IMongoClient>((Func<IMongoClient>)(() => (IMongoClient)new MongoClient(_temporaryMongoDb.Value.ConnectionString)));
        _database = new Lazy<IMongoDatabase>((Func<IMongoDatabase>)(() => mongoClient.Value.GetDatabase(name)));
        MongoDbConfiguration.Setup();
    }

    private readonly Lazy<IMongoRunner> _temporaryMongoDb = new Lazy<IMongoRunner>((Func<IMongoRunner>)(() => MongoRunner.Run(new MongoRunnerOptions
    {
        StandardOuputLogger = (Logger)(_ => { }),
        AdditionalArguments = "--nojournal"
    })));
    private readonly Lazy<IMongoDatabase> _database;
    public IMongoDatabase Database => _database.Value;
}
