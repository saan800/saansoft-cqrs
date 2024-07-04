using MongoDB.Bson;
using SaanSoft.Tests.Cqrs.Common.Utilities;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Utilities;

public class GenericUtilsTestsMongoDb
{
    public class ObjectIdType : GenericUtilsBaseTests.IsNullOrDefault.MessageId<ObjectId>
    {
        protected override ObjectId NewId() => ObjectId.GenerateNewId();
    }
}
