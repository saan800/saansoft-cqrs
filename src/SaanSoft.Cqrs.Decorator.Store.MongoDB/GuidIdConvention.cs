using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public class GuidIdConvention : ConventionBase, IPostProcessingConvention
{
    public void PostProcess(BsonClassMap classMap)
    {
        // Look for the property named "Id" and map it to "_id"
        var idMemberMap = classMap.IdMemberMap;
        if (idMemberMap is { MemberName: "Id" } && idMemberMap.MemberType == typeof(Guid))
        {
            classMap.MapIdProperty("Id").SetIdGenerator(new GuidGenerator());
        }
    }
}
