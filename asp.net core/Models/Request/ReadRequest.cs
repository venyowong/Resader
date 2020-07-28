using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resader.Models.Request
{
    public class ReadRequest
    {
        [Required]
        public List<string> Articles{get;set;}

        [Required]
        public string UserId{get;set;}
    }
}