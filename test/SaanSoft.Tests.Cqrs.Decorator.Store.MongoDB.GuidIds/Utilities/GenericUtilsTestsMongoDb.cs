using MongoDB.Bson;
using SaanSoft.Tests.Cqrs.Common.Utilities;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Utilities;

// TODO: this shouldn't be in the Guid project, but don't have anywhere else to put it yet
public class GenericUtilsTestsMongoDb
{
    public class ObjectIdType : GenericUtilsBaseTests.IsNullOrDefault.ForType<ObjectId>
    {
        protected override ObjectId NewId() => ObjectId.GenerateNewId();
    }
}
