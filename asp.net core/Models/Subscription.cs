using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resader.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("feed_id")]
        public string FeedId { get; set; }

        [Column("create_time")]
        public DateTime CreateTime{get;set;}

        [Column("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}