using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class FeedOverview
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public List<ArticleResponse> Articles { get; set; }
    }
}
