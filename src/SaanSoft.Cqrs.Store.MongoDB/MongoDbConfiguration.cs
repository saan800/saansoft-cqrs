using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public static class MongoDbConfiguration
{
    public static void Setup()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // var objectDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
        // var guidObjectSerializer = new ObjectSerializer(objectDiscriminatorConvention, GuidRepresentation.Standard);
        // BsonSerializer.RegisterSerializer(guidObjectSerializer);

        //ConventionRegistry.Register("NamedId", new ConventionPack{ new NamedIdMemberConvention(["Id"], MemberTypes.Property)}, _ => true);
        ConventionRegistry.Register("IgnoreNull", new ConventionPack { new IgnoreIfNullConvention(true) }, _ => true);
        ConventionRegistry.Register("IgnoreExtras", new ConventionPack { new IgnoreExtraElementsConvention(true) }, _ => true);
        // ConventionRegistry.Register("GuidIdConvention", new ConventionPack { new GuidIdConvention() }, _ => true);

        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.IsAssignableTo(typeof(IMessage)));
        BsonSerializer.RegisterSerializer(objectSerializer);
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
