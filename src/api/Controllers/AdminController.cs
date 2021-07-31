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
        public List<Feed> GetFeeds([FromServices] RssService service) => service.GetFeeds();

        [HttpPost("UpdateFeed")]
        public async Task<bool> UpdateFeed([FromQuery] string id, [FromServices] RssService service)
        {
            var request = (await this.Request.ReadBody()).ToObj<Feed>();
            request.Id = id;
            return await service.UpdateFeed(request);
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
