using MongoDB.Driver;
using TinyUrlApi.Models;

namespace TinyUrlApi.Data
{
    public interface ITinyUrlContext
    {
        IMongoCollection<MongoUrlEntity> TinyUrlCollection { get;}
    }
}
