using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Resader.Common.Entities
{
    public class Feed
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool Recommend { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
