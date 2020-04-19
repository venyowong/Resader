namespace Resader.Host.Models
{
    public static class Converter
    {
        public static Resader.Grains.Models.Article ToResponseModel(this Article article)
        {
            if (article == null)
            {
                return null;
            }

            return new Resader.Grains.Models.Article
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

        public static Resader.Grains.Models.Feed ToResponseModel(this Feed feed)
        {
            if (feed == null)
            {
                return null;
            }

            return new Resader.Grains.Models.Feed
            {
                Id = feed.Id,
                Url = feed.Url,
                Title = feed.Title
            };
        }
    }
}