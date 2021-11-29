using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Resader.Common.Api.Request
{
    public class ResetPasswordRequest
    {
        public string OldPassword { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
