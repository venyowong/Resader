using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resader.Models.Request
{
    public class UnsubscribeRequest
    {
        [Required]
        public List<string> Feeds{get;set;}

        [Required]
        public string UserId{get;set;}
    }
}