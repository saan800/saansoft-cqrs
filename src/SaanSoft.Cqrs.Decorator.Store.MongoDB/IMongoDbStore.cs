using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IMongoDbStore<TMessageId> where TMessageId : struct
{
    IMongoCollection<IMessage<TMessageId>> MessageCollection { get; }
}
