using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Resader.Common.Entities
{
    public class User
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        /// <summary>
        /// 角色 0 admin 1 普通用户
        /// </summary>
        public int Role { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
