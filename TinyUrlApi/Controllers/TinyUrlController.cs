using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TinyUrlApi.Data;
using TinyUrlApi.Helpers;
using TinyUrlApi.Models;

namespace TinyUrlApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinyUrlController : ControllerBase
    {
        private ITinyUrlRepository _tinyUrlRepository;

        public TinyUrlController(ITinyUrlRepository tinyUrlRepository)
        {
            _tinyUrlRepository = tinyUrlRepository;
        }

        [HttpGet("{shortUrl}")]
        public ActionResult<UrlEntity> Get(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl))
            {
                return BadRequest();
            }
            var mongoUrlEntity = _tinyUrlRepository.GetLongUrl(DataConvertor.ToLongN(shortUrl));
            if (mongoUrlEntity == null)
            {
                return NotFound();
            }
            return new ObjectResult(new UrlEntity()
            {
                ShortUrl = shortUrl,
                LongUrl = mongoUrlEntity.LongUrl
            });
        }

        [HttpPost]
        public ActionResult<UrlEntity> Post([FromBody] string longUrl)
        {
            var shortIdCounter = Global.GetCounter();

            var shortId = long.Parse(Global.Prefix + "" + shortIdCounter);
            var shortUrl = DataConvertor.ToBaseN(shortId);

            var mongoUrlEntity = _tinyUrlRepository.PostLongUrl(new MongoUrlEntity()
            {
                LongUrl = longUrl,
                ShortUrlId = new BsonInt64(shortId),
                ServerPrefix = Global.Prefix
            });

            var urlEntity = new UrlEntity()
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl
            };
            return urlEntity;
        }
    }
}