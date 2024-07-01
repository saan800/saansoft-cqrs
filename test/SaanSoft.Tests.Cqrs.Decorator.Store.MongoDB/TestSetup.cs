using System.Reflection;
using EphemeralMongo;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB;

public class TestSetup : SaanSoft.Tests.Cqrs.Common.TestSetup
{
    protected TestSetup() : base()
    {
        var assembly = typeof(TestSetup).Assembly;
        var name = assembly.GetName().FullName;
        Lazy<IMongoClient> mongoClient = new(() => new MongoClient(_temporaryMongoDb.Value.ConnectionString));
        _database = new Lazy<IMongoDatabase>(() => mongoClient.Value.GetDatabase(name));
        MongoDbConfiguration.Setup(new MongoDbConfigurationOptions
        {
            ConfigureGuidId = true,
            CamelCaseElementName = true,
            RegisterMessageClassMapForAssemblies = new List<Assembly>
            {
                assembly,
                typeof(MyCommand).Assembly
            }
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
