using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrlApi.Data
{
    public class MongoUrlEntity
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public string LongUrl { get; set;}
        public BsonInt64 ShortUrlId { get; set;}
        public int ServerPrefix { get; set; }
    }
}
