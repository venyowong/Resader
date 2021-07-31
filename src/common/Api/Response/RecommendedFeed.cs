using Resader.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class RecommendedFeed
    {
        public Feed Feed { get; set; }

        public Article Article { get; set; }
    }
}
