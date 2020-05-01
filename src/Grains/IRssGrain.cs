using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using Resader.Grains.Models;

namespace Resader.Grains
{
    public interface IRssGrain : IGrainWithStringKey
    {
        [Authorize]
        [HttpPost("{grainId}/rss/subscribe")]
        Task<Result<List<FeedOverview>>> Subscribe([FromBody] List<string> feeds);

        [Authorize]
        [HttpGet("{grainId}/rss/feeds")]
        Task<Result<List<Feed>>> GetFeeds();

        [Authorize]
        [HttpGet("{grainId}/rss/articles")]
        Task<Result<List<Article>>> GetArticles([FromQuery] string feedId, [FromQuery] int page, 
            [FromQuery] int pageCount, [FromQuery] string endTime);

        [Authorize]
        [HttpPost("{grainId}/rss/unsubscribe")]
        Task<Result> Unsubscribe([FromBody] List<string> feeds);

        [Authorize]
        [HttpPost("{grainId}/article/read")]
        Task<Result> Read([FromBody] List<string> articles);

        [HttpGet("{grainId}/rss/opml.xml")]
        Task<string> GetOpml();

        [Authorize]
        [HttpGet("{grainId}/rss/activeFeeds")]
        Task<Result<List<string>>> GetActiveFeeds();
    }
}