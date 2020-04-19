using System;
using System.ComponentModel;

namespace Resader.Host.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public string Salt{get;set;}

        [Description("create_time")]
        public DateTime CreateTime{get;set;}

        [Description("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}