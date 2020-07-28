using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resader.Models
{
    public class Feed
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        [Column("create_time")]
        public DateTime CreateTime{get;set;}

        [Column("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}