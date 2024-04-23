using System.Reflection;

namespace SaanSoft.Cqrs.Store.MongoDB;

public class MongoDbConfigurationOptions
{
    public bool ConfigureGuidSerialisation { get; set; } = true;

    public bool ConfigureGuidId { get; set; } = true;

    public bool ConfigureObjectId { get; set; } = true;

    public bool IgnoreNulls { get; set; } = true;

    public bool IgnoreExtraElements { get; set; } = true;

    /// <summary>
    /// RegisterClassMap for all Commands / Events / Queries in these assemblies.
    /// Automatically adds typeof(Command).Assembly to the list
    /// </summary>
    public IList<Assembly> RegisterClassMapForAssemblies { get; set; } = new List<Assembly>();
}
