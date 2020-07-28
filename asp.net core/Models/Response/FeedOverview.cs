using System.Collections.Generic;

namespace Resader.Models.Response
{
    public class FeedOverview
    {
        public string Id{get;set;}
        
        public string Title { get; set; }

        public List<ArticleResponse> Articles { get; set; }
    }
}