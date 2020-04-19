using System;
using System.ComponentModel;

namespace Resader.Host.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Description("user_id")]
        public string UserId { get; set; }

        [Description("feed_id")]
        public string FeedId { get; set; }

        [Description("create_time")]
        public DateTime CreateTime{get;set;}

        [Description("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}