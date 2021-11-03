using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Resader.Common.Entities
{
    public class ReadRecord
    {
        [Column("article_id")]
        public string ArticleId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
