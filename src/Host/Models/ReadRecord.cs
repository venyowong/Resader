using System;
using System.ComponentModel;

namespace Resader.Host.Models
{
    public class ReadRecord
    {
        [Description("article_id")]
        public string ArticleId{get;set;}

        [Description("user_id")]
        public string UserId{get;set;}

        [Description("create_time")]
        public DateTime CreateTime{get;set;}

        [Description("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}