using System.ComponentModel.DataAnnotations;

namespace Resader.Models.Request
{
    public class SignUpRequest
    {
        [Required]
        public string Mail{get;set;}

        [Required]
        public string Password{get;set;}
    }
}