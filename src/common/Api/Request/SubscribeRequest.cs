using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Resader.Common.Api.Request
{
    public class SubscribeRequest
    {
        [Required]
        public List<string> Feeds { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
