using Microsoft.AspNetCore.Mvc;
using Resader.Api.Attributes;
using Resader.Api.Daos;
using Resader.Api.Extensions;
using Resader.Api.Services;
using Resader.Common.Api.Response;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Controllers
{
    [ApiController]
    [Route("/Admin")]
    [JwtValidation(0)]
    public class AdminController : BaseController
    {
        [HttpGet("Feeds")]
        public List<Feed> GetFeeds([FromQuery] int page, [FromQuery] int perPage, [FromServices] RssService service)
        {
            return service.GetFeeds()
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToList();
        }

        [HttpPost("UpdateFeed")]
        public async Task<bool> UpdateFeed([FromQuery] string id, [FromServices] RssService service)
        {
            var request = (await this.Request.ReadBody()).ToObj<Feed>();
            request.Id = id;
            request.UpdateTime = DateTime.Now;
            return await service.UpdateFeed(request);
        }

        [HttpPost("AddFeed")]
        public async Task<bool> AddFeed([FromQuery] string url, [FromServices] RssService service)
        {
            await service.AddFeed(url);
            return true;
        }

        [HttpGet("Labels")]
        public List<string> GetLabels([FromServices] RecommendService recommendService) => recommendService.GetLabels();

        [HttpGet("LabeledFeeds")]
        public List<Feed> GetLabeledFeeds(string label, [FromServices] RecommendService recommendService) => recommendService.GetLabeledFeeds(label);

        [HttpPost("RecommendFeed")]
        public Task<bool> RecommendFeed([FromQuery] string label, [FromQuery] string feedId,
            [FromServices] RecommendService recommendService) => recommendService.RecommendFeed(label, feedId);

        [HttpPost("DerecommendFeed")]
        public Task<bool> DerecommendFeed([FromQuery] string label, [FromQuery] string feedId,
            [FromServices] RecommendService recommendService) => recommendService.DerecommendFeed(label, feedId);
    }
}
