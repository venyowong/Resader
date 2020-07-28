using System.ComponentModel.DataAnnotations;

namespace Resader.Models.Request
{
    public class GetArticlesRequest
    {
        [Required]
        public string FeedId{get;set;}

        [Range(0, int.MaxValue)]
        public int Page{get;set;}

        [Range(1, int.MaxValue)]
        public int PageCount{get;set;}

        public string EndTime{get;set;}

        [Required]
        public string UserId{get;set;}
    }
}