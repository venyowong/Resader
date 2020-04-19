using System;
using System.ComponentModel;

namespace Resader.Host.Models
{
    public class Article
    {
        public string Id { get; set; }

        public string Url { get; set; }

        [Description("feed_id")]
        public string FeedId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTime Published { get; set; }

        public DateTime Updated { get; set; }

        public DateTime Created { get; set; }

        public string Keyword { get; set; }

        public string Content { get; set; }

        public string Contributors { get; set; }

        public string Authors { get; set; }

        public string Copyright { get; set; }

        [Description("create_time")]
        public DateTime CreateTime{get;set;}

        [Description("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}