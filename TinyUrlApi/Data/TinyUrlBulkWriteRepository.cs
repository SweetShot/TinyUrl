using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TinyUrlApi.Helpers;

namespace TinyUrlApi.Data
{
    public class TinyUrlBulkWriteRepository : ITinyUrlRepository
    {
        private ITinyUrlContext _context;

        public TinyUrlBulkWriteRepository(ITinyUrlContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var ops = new List<WriteModel<MongoUrlEntity>>();

            ops.Add(new InsertOneModel<MongoUrlEntity>(new MongoUrlEntity
            {
                LongUrl = "t1", ServerPrefix = Global.Prefix, ShortUrlId = new BsonInt64(Global.GetCounter())
            }));
            ops.Add(new InsertOneModel<MongoUrlEntity>(new MongoUrlEntity
            {
                LongUrl = "t2",
                ServerPrefix = Global.Prefix,
                ShortUrlId = new BsonInt64(Global.GetCounter())
            }));
            ops.Add(new InsertOneModel<MongoUrlEntity>(new MongoUrlEntity
            {
                LongUrl = "t3",
                ServerPrefix = Global.Prefix,
                ShortUrlId = new BsonInt64(Global.GetCounter())
            }));
            ops.Add(new InsertOneModel<MongoUrlEntity>(new MongoUrlEntity
            {
                LongUrl = "t4",
                ServerPrefix = Global.Prefix,
                ShortUrlId = new BsonInt64(Global.GetCounter())
            }));
            ops.Add(new InsertOneModel<MongoUrlEntity>(new MongoUrlEntity
            {
                LongUrl = "t5",
                ServerPrefix = Global.Prefix,
                ShortUrlId = new BsonInt64(Global.GetCounter())
            }));

            var ans = _context.TinyUrlCollection.BulkWrite(ops);

            Console.WriteLine();
        }

        public MongoUrlEntity GetLongUrl(long shortUrlId)
        {
            throw new NotImplementedException();
        }

        public MongoUrlEntity PostLongUrl(MongoUrlEntity longMongoUrl)
        {
            throw new NotImplementedException();
        }

        public Task<MongoUrlEntity> GetLongUrlAsync(long shortUrlId)
        {
            throw new NotImplementedException();
        }

        public Task PostLongUrlAsync(MongoUrlEntity longMongoUrl)
        {
            throw new NotImplementedException();
        }
    }
}
