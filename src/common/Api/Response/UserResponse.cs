using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class UserResponse
    {
        public string Id { get; set; }

        public string Mail { get; set; }

        public string Token { get; set; }

        public int Role { get; set; }

        /// <summary>
        /// 三方 id
        /// </summary>
        public string OauthId { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string Source { get; set; }

        public string Url { get; set; }

        public string Location { get; set; }

        public string Company { get; set; }

        public string Blog { get; set; }

        public string Bio { get; set; }
    }
}
