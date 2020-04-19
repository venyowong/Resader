using System.Collections.Generic;

namespace Resader.Grains.Models
{
    public class FeedOverview
    {
        public string Id{get;set;}
        
        public string Title { get; set; }

        public List<Article> Articles { get; set; }
    }
}