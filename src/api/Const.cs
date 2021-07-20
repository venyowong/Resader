using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api
{
    public static class Const
    {
        /// <summary>
        /// feed 最新更新时间
        /// </summary>
        public const string FeedLatestTimeCache = "Resader:Api:Feed:LatestTime:";

        /// <summary>
        /// feed 下所有 article
        /// </summary>
        public const string ArticlesInFeedCache = "Resader:Api:Feed:Articles:";

        /// <summary>
        /// 用户阅读记录
        /// </summary>
        public const string ReadRecordCache = "Resader:Api:ReadRecords:";

        public const string FeedCache = "Resader:Api:Feeds:";

        /// <summary>
        /// 用户最后一次浏览 feed 的时间
        /// </summary>
        public const string FeedLatestBrowseTimeCache = "Resader:Api:Feed:LatestBrowseTime:";
    }
}
