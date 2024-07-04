using EphemeralMongo;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds;

public class TestSetup : SaanSoft.Tests.Cqrs.Common.TestSetup
{
    protected TestSetup() : base()
    {
        var assembly = typeof(TestSetup).Assembly;
        var name = typeof(TestSetup).GetAssemblyName().Replace(".", "");
        Lazy<IMongoClient> mongoClient = new(() => new MongoClient(_temporaryMongoDb.Value.ConnectionString));
        _database = new Lazy<IMongoDatabase>(() => mongoClient.Value.GetDatabase(name));
        MongoDbConfiguration.Setup(new MongoDbConfigurationOptions
        {
            ConfigureGuidId = true,
            CamelCaseElementName = true,
            RegisterMessageClassMapForAssemblies =
            [
                assembly,
                typeof(MyCommand).Assembly
            ]
        });
    }

    private readonly Lazy<IMongoRunner> _temporaryMongoDb = new Lazy<IMongoRunner>((Func<IMongoRunner>)(() => MongoRunner.Run(new MongoRunnerOptions
    {
        StandardOuputLogger = (Logger)(_ => { }),
        AdditionalArguments = "--nojournal"
    })));

    private readonly Lazy<IMongoDatabase> _database;
    protected IMongoDatabase Database => _database.Value;
}
