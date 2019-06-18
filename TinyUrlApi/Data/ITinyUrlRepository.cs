using System.Threading.Tasks;
using TinyUrlApi.Models;

namespace TinyUrlApi.Data
{
    public interface ITinyUrlRepository
    {
        MongoUrlEntity GetLongUrl(long shortUrlId);
        MongoUrlEntity PostLongUrl(MongoUrlEntity longMongoUrl);
        Task<MongoUrlEntity> GetLongUrlAsync(long shortUrlId);
        Task PostLongUrlAsync(MongoUrlEntity longMongoUrl);
    }
}
