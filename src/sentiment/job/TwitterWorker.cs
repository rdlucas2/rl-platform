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
                    var usaWoeId = 23424977;
                    var trendingSearches = await _twitterClient.Trends.GetPlaceTrendsAtAsync(new GetTrendsAtParameters(usaWoeId));
                    long numberOneTrending = 0;
                    int? trendingIndex = null;
                    for(int i = 0; i < trendingSearches.Trends.Length; i++)
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
                    if(!trendingIndex.HasValue)
                    {
                        _logger.LogInformation("No trending searches found");
                        continue;
                    }
                    var trendName = trendingSearches.Trends[trendingIndex.Value].Name;
                    var searchParameters = new SearchTweetsParameters(trendName);
                    searchParameters.Lang = LanguageFilter.English;

                    var tweets = await _twitterClient.Search.SearchTweetsAsync(searchParameters);
                    foreach(var tweet in tweets.Where(x => !x.IsRetweet))
                    {
                        _logger.LogInformation("Id: {id}, Trend: {trend}, Body: {text}", tweet.IdStr, trendName, tweet.FullText);
                        using (HttpClient httpClient = new HttpClient())
                        {
                            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
                            if(daprPort == null)
                                daprPort = "3500";
                            var result = await httpClient.PostAsJsonAsync($"http://localhost:{daprPort}/v1.0/invoke/fastapiapp/method/evaluate", new {
                                id = tweet.IdStr,
                                text = tweet.FullText,
                                trend = trendName
                            });
                            _logger.LogInformation("Response: {response}", await result.Content.ReadAsStringAsync());
                        }
                    }
                } 
                catch (TwitterException  e) 
                {
                    if(e.StatusCode == 429)
                        _logger.LogInformation("Rate limit exceeded.");
                    else
                    {
                        _logger.LogError(e.Message);
                        throw e;
                    }
                }
                finally 
                {
                    //await Task.Delay(Convert.ToInt32(Environment.GetEnvironmentVariable("RetryFrequencyInMs")), stoppingToken);
                    await Task.Delay(60000, stoppingToken);
                }
            }
        }
    }
}
