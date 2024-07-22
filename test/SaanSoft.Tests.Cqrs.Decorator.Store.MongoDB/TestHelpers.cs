using EphemeralMongo;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB;

public static class TestHelpers
{
    private static readonly object Padlock = new object();
    private static IMongoRunner? _temporaryMongoDb = null;

    private static IMongoRunner GetMongoRunner()
    {
        lock (Padlock)
        {
            if (_temporaryMongoDb == null)
            {
                var assembly = typeof(TestHelpers).Assembly;
                _temporaryMongoDb = MongoRunner.Run(new MongoRunnerOptions
                {
                    StandardOuputLogger = (Logger)(_ => { }),
                    AdditionalArguments = "--nojournal"
                });
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
            return _temporaryMongoDb;
        }
    }

    public static IMongoDatabase GetDatabase()
    {
        var name = typeof(TestHelpers).GetAssemblyName().Replace(".", "");
        Lazy<IMongoClient> mongoClient = new(() => new MongoClient(GetMongoRunner().ConnectionString));
        var database = mongoClient.Value.GetDatabase(name);
        return database;
    }
}
