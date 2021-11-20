using Resader.Api.Daos;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Services;

public class RecommendService
{
    private ICacheService cache;
    private RecommendDao dao;

    public RecommendService(ICacheService cache, RecommendDao dao)
    {
        this.cache = cache;
        this.dao = dao;
    }

    public void UpdateFeedLabel(Feed feed, string oldLabel)
    {
        if (string.IsNullOrWhiteSpace(feed?.Label))
        {
            return;
        }

        var recommendLabels = new List<string>();
        if (!string.IsNullOrWhiteSpace(oldLabel))
        {
            foreach (var label in oldLabel.Split(',', '，', ';', '；'))
            {
                var f = this.GetLabeledFeed(label, feed.Id);
                if (f != null && f.Recommend)
                {
                    recommendLabels.Add(label);
                }
                this.cache.HashDelete(Const.LabeledFeedsCache + label, feed.Id);
            }
        }

        if (!string.IsNullOrWhiteSpace(feed.Label))
        {
            var labels = this.GetLabels();
            foreach (var label in feed.Label.Split(',', '，', ';', '；'))
            {
                if (!labels.Contains(label))
                {
                    labels.Add(label);
                }

                if (recommendLabels.Contains(label))
                {
                    feed.Recommend = true;
                }
                else
                {
                    feed.Recommend = false;
                }
                this.cache.HashSet(Const.LabeledFeedsCache + label, feed.Id, feed.ToJson());
            }

            this.cache.StringSet(Const.LabelsCache, labels.ToJson().GZipCompress());
        }
    }

    public async Task RefreshLabels(List<Feed> feeds)
    {
        if (feeds.IsNullOrEmpty())
        {
            return;
        }

        var recommends = await this.dao.GetRecommends();
        var labels = new List<string>();
        foreach (var feed in feeds)
        {
            if (string.IsNullOrWhiteSpace(feed.Label))
            {
                continue;
            }

            foreach (var label in feed.Label.Split(',', '，', ';', '；'))
            {
                if (!labels.Contains(label))
                {
                    labels.Add(label);
                }

                if (recommends.Any(x => x.FeedId == feed.Id && x.Label == label))
                {
                    feed.Recommend = true;
                }
                else
                {
                    feed.Recommend = false;
                }
                this.cache.HashSet(Const.LabeledFeedsCache + label, feed.Id, feed.ToJson());
            }
        }

        this.cache.StringSet(Const.LabelsCache, labels.ToJson().GZipCompress());
    }

    public List<string> GetLabels()
    {
        var result = this.cache.StringGet(Const.LabelsCache).GZipDecompress().ToObj<List<string>>();
        if (result == null)
        {
            result = new List<string>();
        }
        return result;
    }

    public Feed GetLabeledFeed(string label, string feedId) => this.cache.HashGet(Const.LabeledFeedsCache + label, feedId).ToObj<Feed>();

    public List<Feed> GetLabeledFeeds(string label) =>
        this.cache.HashGetAll(Const.LabeledFeedsCache + label)
            .Select(p => p.Value.ToObj<Feed>())
            .ToList();

    public async Task<bool> RecommendFeed(string label, string feedId)
    {
        var feed = this.GetLabeledFeed(label, feedId);
        if (feed == null)
        {
            return false;
        }

        if (feed.Recommend)
        {
            return true;
        }

        if (!await this.dao.InsertRecommend(label, feedId))
        {
            return false;
        }

        feed.Recommend = true;
        this.cache.HashSet(Const.LabeledFeedsCache + label, feed.Id, feed.ToJson());
        return true;
    }

    public async Task<bool> DerecommendFeed(string label, string feedId)
    {
        var feed = this.GetLabeledFeed(label, feedId);
        if (feed == null)
        {
            return false;
        }

        if (!feed.Recommend)
        {
            return true;
        }

        if (!await this.dao.DeleteRecommend(label, feedId))
        {
            return false;
        }

        feed.Recommend = false;
        this.cache.HashSet(Const.LabeledFeedsCache + label, feed.Id, feed.ToJson());
        return true;
    }
}
