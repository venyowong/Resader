using System;
using System.ComponentModel;

namespace Resader.Host.Models
{
    public class Feed
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        [Description("create_time")]
        public DateTime CreateTime{get;set;}

        [Description("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}