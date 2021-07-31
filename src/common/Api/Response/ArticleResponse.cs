﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class ArticleResponse
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string FeedId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTime Published { get; set; }

        public DateTime Updated { get; set; }

        public string Keyword { get; set; }

        public string Content { get; set; }

        public string Contributors { get; set; }

        public string Authors { get; set; }

        public string Copyright { get; set; }

        public bool Read { get; set; }
    }
}
