using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Parameters.TrendsClient;

namespace job
{
    public class TwitterWorker : BackgroundService
    {
        private string DAPR_PORT = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
        private int RETRY_FREQUENCY_IN_MS = Convert.ToInt32(Environment.GetEnvironmentVariable("RetryFrequencyInMs"));
        private const int USA_WOE_ID = 23424977;
        private readonly ILogger<TwitterWorker> _logger;
        private readonly TwitterClient _twitterClient;

        public TwitterWorker(ILogger<TwitterWorker> logger, TwitterClient twitterClient)
        {
            _logger = logger;
            _twitterClient = twitterClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    var trendName = await GetUnitedStatesTrendingTweetTopic();
                    var searchParameters = new SearchTweetsParameters(trendName);
                    searchParameters.Lang = LanguageFilter.English;
                    var tweets = await GetTweets(searchParameters);
                    await SaveTweetsByTrend(tweets, trendName);
                }
                catch(Exception ex)
                {
                    _logger.LogError(JobLogEvents.UnknownError, ex, "Error occurred while getting tweets");
                }
                finally
                {
                    await Task.Delay(RETRY_FREQUENCY_IN_MS, stoppingToken);
                }
            }
        }

        private async Task<string> GetUnitedStatesTrendingTweetTopic()
        {
            try
            {
                var trendingSearches = await _twitterClient.Trends.GetPlaceTrendsAtAsync(new GetTrendsAtParameters(USA_WOE_ID));
                long numberOneTrending = 0;
                int? trendingIndex = null;
                for (int i = 0; i < trendingSearches.Trends.Length; i++)
                {
                    if (
                        trendingSearches.Trends[i].TweetVolume.HasValue &&
                        trendingSearches.Trends[i].TweetVolume.Value >= numberOneTrending
                    )
                    {
                        numberOneTrending = trendingSearches.Trends[i].TweetVolume.Value;
                        trendingIndex = i;
                    }
                }
                if (!trendingIndex.HasValue)
                {
                    _logger.LogError(JobLogEvents.TwitterTrendNotFound, "No trending topics found");
                    return null;
                }

                return trendingSearches.Trends[trendingIndex.Value].Name;
            }
            catch (TwitterException ex)
            {
                if (ex.StatusCode == 429)
                    _logger.LogError(JobLogEvents.TwitterRateLimitExceeded, ex, "Twitter rate limit exceeded");
                else
                    _logger.LogError(JobLogEvents.TwitterSearchNotFound, ex, "Search tweets failed.");

                return null;
            }
        }

        private async Task<ITweet[]> GetTweets(SearchTweetsParameters searchTweetsParameters)
        {
            try
            {
                return await _twitterClient.Search.SearchTweetsAsync(searchTweetsParameters);
            }
            catch (TwitterException ex)
            {
                if (ex.StatusCode == 429)
                    _logger.LogError(JobLogEvents.TwitterRateLimitExceeded, ex, "Twitter rate limit exceeded");
                else
                    _logger.LogError(JobLogEvents.TwitterSearchNotFound, ex, "Search tweets failed.");

                return null;
            }
        }

        private async Task SaveTweetsByTrend(ITweet[] tweets, string trendName)
        {
            foreach (var tweet in tweets.Where(x => !x.IsRetweet))
            {
                await SaveTweetByTrend(tweet, trendName);
            }
        }

        private async Task SaveTweetByTrend(ITweet tweet, string trendName)
        {
            _logger.LogInformation("Id: {id}, Trend: {trend}, Body: {text}", tweet.IdStr, trendName, tweet.FullText);
            using (HttpClient httpClient = new HttpClient())
            {
                var result = await httpClient.PostAsJsonAsync($"http://localhost:{DAPR_PORT}/v1.0/invoke/fastapiapp/method/evaluate", new
                {
                    id = tweet.IdStr,
                    text = tweet.FullText,
                    trend = trendName
                });
                _logger.LogInformation("Response: {response}", await result.Content.ReadAsStringAsync());
            }
        }
    }
}
