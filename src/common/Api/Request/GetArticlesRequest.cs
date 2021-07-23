using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Resader.Common.Api.Request
{
    public class GetArticlesRequest
    {
        [Required]
        public string FeedId { get; set; }

        [Range(0, int.MaxValue)]
        public int Page { get; set; }

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }

        public string EndTime { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
