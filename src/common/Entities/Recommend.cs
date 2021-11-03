using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Resader.Common.Entities
{
    public class Recommend
    {
        public string Label { get; set; }

        [Column("feed_id")]
        public string FeedId { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
