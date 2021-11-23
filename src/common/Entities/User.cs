using Newtonsoft.Json;
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

        /// <summary>
        /// 三方 id
        /// </summary>
        [Column("oauth_id")]
        public string OauthId { get; set; }

        public string Name { get; set; }

        [Column("avatar_url")]
        public string AvatarUrl { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }

        public string Location { get; set; }

        public string Company { get; set; }

        public string Blog { get; set; }

        public string Bio { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
