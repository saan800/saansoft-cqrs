using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public static class MongoDbConfiguration
{
    private static bool _hasAlreadyRun;

    /// <summary>
    /// Configuration for MongoDB message stores (and some MongoDB things in general)
    /// Should only be run once on startup
    /// </summary>
    /// <param name="options">@default MongoDbConfigurationOptions</param>
    public static void Setup(MongoDbConfigurationOptions? options = null)
    {
        if (_hasAlreadyRun) return;
        _hasAlreadyRun = true;

        options ??= new MongoDbConfigurationOptions();
        if (options.ConfigureGuidSerialisation)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        if (options.IgnoreNulls) ConventionRegistry.Register("IgnoreNull", new ConventionPack { new IgnoreIfNullConvention(true) }, _ => true);
        if (options.IgnoreExtraElements) ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, _ => true);
        if (options.ConfigureGuidId) ConventionRegistry.Register("GuidIdConvention", new ConventionPack { new GuidIdConvention() }, _ => true);
        if (options.ConfigureObjectId) ConventionRegistry.Register("ObjectIdIdConvention", new ConventionPack { new StringObjectIdIdGeneratorConvention() }, _ => true);

        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.IsAssignableTo(typeof(IMessage)));
        BsonSerializer.RegisterSerializer(objectSerializer);

        RegisterClassMaps(options.RegisterClassMapForAssemblies);
    }

    /// <summary>
    /// Register all ClassMaps for classes that extend IMessage in the list of provided assemblies
    /// Automatically adds typeof(Command).Assembly to list
    /// </summary>
    /// <param name="assemblies"></param>
    public static void RegisterClassMaps(IEnumerable<Assembly>? assemblies)
    {
        var baseMessageAssembly = typeof(Command).Assembly;
        var assemblyList = assemblies?.ToList() ?? [];

        if (!assemblyList.Contains(baseMessageAssembly))
        {
            assemblyList.Add(baseMessageAssembly);
        }

        foreach (var t in assemblyList
                     .SelectMany(assembly => assembly.GetExportedTypes())
                     .Where(t => t is { IsAbstract: false, IsClass: true }
                                 && (typeof(IMessage).IsAssignableFrom(t))
                     ))
        {
            if (!BsonClassMap.IsClassMapRegistered(t))
            {
                var classMapDefinition = typeof(BsonClassMap<>);
                var classMapType = classMapDefinition.MakeGenericType(t);
                var classMap = (BsonClassMap)Activator.CreateInstance(classMapType)!;
                classMap.AutoMap();
                BsonClassMap.RegisterClassMap(classMap);
            }
        }
    }
}
