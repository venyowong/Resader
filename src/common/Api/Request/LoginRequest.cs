using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Resader.Common.Api.Request
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Mail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
