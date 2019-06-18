using MongoDB.Bson;
using MongoDB.Driver;
using TinyUrlApi.Config;
using TinyUrlApi.Helpers;

namespace TinyUrlApi.Data
{
    public class TinyUrlContext : ITinyUrlContext
    {
        private string _collectionName = "TinyUrls";

        public IMongoCollection<MongoUrlEntity> TinyUrlCollection { get; }

        public TinyUrlContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            var db = client.GetDatabase(config.Database);

            if (!CollectionExists(db, _collectionName))
            {
                db.CreateCollection(_collectionName);
                TinyUrlCollection = db.GetCollection<MongoUrlEntity>(_collectionName);
            }
            else // if it already exists set new initial counter value   
            {
                TinyUrlCollection = db.GetCollection<MongoUrlEntity>(_collectionName);
                var builder = Builders<MongoUrlEntity>.Sort;
                var sort = builder.Descending("ShortUrlId");
                var doc = TinyUrlCollection.Find(entity => entity.ServerPrefix == Global.Prefix).Sort(sort).FirstOrDefault();
                if (doc != null)
                    Global.SetCounter(long.Parse(doc.ShortUrlId.ToInt64().ToString().Substring(3)));
            }
        }

        public bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };

            return database.ListCollectionNames(options).Any();
        }

    }
}