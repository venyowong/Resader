using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resader.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public string Salt{get;set;}

        [Column("create_time")]
        public DateTime CreateTime{get;set;}

        [Column("update_time")]
        public DateTime UpdateTime{get;set;}
    }
}