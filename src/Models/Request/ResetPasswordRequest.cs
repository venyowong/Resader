using System.ComponentModel.DataAnnotations;

namespace Resader.Models.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string OldPassword{get;set;}

        [Required]
        public string Password{get;set;}

        [Required]
        public string UserId{get;set;}
    }
}