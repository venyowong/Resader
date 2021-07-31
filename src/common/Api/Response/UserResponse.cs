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
    }
}
