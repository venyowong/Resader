using Resader.Common.Api.Response;
using Resader.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common
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
                Keyword = article.Keyword,
                Content = article.Content,
                Contributors = article.Contributors,
                Authors = article.Authors,
                Copyright = article.Copyright
            };
        }
    }
}
