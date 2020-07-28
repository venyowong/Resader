using Resader.Models.Response;

namespace Resader.Models
{
    public static class Converter
    {
        public static ArticleResponse ToResponseModel(this Article article)
        {
            if (article == null)
            {
                return null;
            }

            return new ArticleResponse
            {
                Id = article.Id,
                Url = article.Url,
                FeedId = article.FeedId,
                Title = article.Title,
                Summary = article.Summary,
                Published = article.Published,
                Updated = article.Updated,
                Created = article.Created,
                Keyword = article.Keyword,
                Content = article.Content,
                Contributors = article.Contributors,
                Authors = article.Authors,
                Copyright = article.Copyright
            };
        }
    }
}