using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class FeedResponse
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }
        
        /// <summary>
        /// 是否有新更新的文章
        /// </summary>
        public bool Active { get; set; }

        public int NewArticleCount { get; set; }
    }
}
