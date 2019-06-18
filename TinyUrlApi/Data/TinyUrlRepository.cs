using System.Threading.Tasks;
using MongoDB.Driver;

namespace TinyUrlApi.Data
{
    public class TinyUrlRepository : ITinyUrlRepository
    {
        private ITinyUrlContext _context;

        public TinyUrlRepository(ITinyUrlContext context)
        {
            _context = context;
        }

        public MongoUrlEntity GetLongUrl(long shortUrlId)
        {
            return _context.TinyUrlCollection.Find(url => url.ShortUrlId == shortUrlId).FirstOrDefault();
        }

        public MongoUrlEntity PostLongUrl(MongoUrlEntity longMongoUrl)
        {
            _context.TinyUrlCollection.InsertOne(longMongoUrl);
            return longMongoUrl;
        }

        public Task<MongoUrlEntity> GetLongUrlAsync(long shortUrlId)
        {
            return _context.TinyUrlCollection.Find(url => url.ShortUrlId == shortUrlId).FirstOrDefaultAsync();
        }

        public Task PostLongUrlAsync(MongoUrlEntity mongoUrlEntity)
        {
            return _context.TinyUrlCollection.InsertOneAsync(mongoUrlEntity);
        }
    }
}
