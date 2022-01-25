using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;

namespace altjob
{
    class Program
    {
        private static readonly string DAPR_PORT = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            var twitterSettings = config.GetSection("Twitter").Get<TwitterSettings>();

            var creds = new ConsumerOnlyCredentials(twitterSettings.ConsumerKey, twitterSettings.ConsumerSecret)
            {
                BearerToken = twitterSettings.Bearer
            };

            var twitterClient = new TwitterClient(creds);
            twitterClient.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            //await StreamTweetsV1(twitterClient);
            await StreamTweetsV2(twitterClient);
        }

        private static async Task StreamTweetsV2(TwitterClient twitterClient)
        {
            var sampleStreamV2 = twitterClient.StreamsV2.CreateSampleStream();

            sampleStreamV2.TweetReceived += async (sender, args) =>
            {
                try
                {
                    await SaveTweetByTrend(args.Tweet);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            };

            //var streamParams = new StartSampleStreamV2Parameters();
            //await sampleStreamV2.StartAsync(streamParams);

            await sampleStreamV2.StartAsync();
        }

        private static async Task StreamTweetsV1(TwitterClient twitterClient)
        {
            var stream = twitterClient.Streams.CreateFilteredStream();

            stream.AddTrack("dotnet");

            stream.MatchingTweetReceived += async (sender, eventReceived) =>
            {
                if (!eventReceived.Tweet.IsRetweet)
                    await SaveTweetByTrend(eventReceived.Tweet);
            };

            await stream.StartMatchingAllConditionsAsync();
        }

        private static async Task SaveTweetByTrend(ITweet tweet)
        {
            Console.WriteLine($"Id: {tweet.IdStr}, Body: {tweet.FullText}");
            using (HttpClient httpClient = new HttpClient())
            {
                var result = await httpClient.PostAsJsonAsync($"http://localhost:{DAPR_PORT}/v1.0/invoke/fastapiapp/method/evaluate", new
                {
                    id = tweet.IdStr,
                    text = tweet.FullText,
                    trend = "altjob"
                });
                var response = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {response}");
            }
        }

        private static async Task SaveTweetByTrend(TweetV2 tweet)
        {
            Console.WriteLine($"Id: {tweet.Id}, Body: {tweet.Text}");
            using (HttpClient httpClient = new HttpClient())
            {
                var result = await httpClient.PostAsJsonAsync($"http://localhost:{DAPR_PORT}/v1.0/invoke/fastapiapp/method/evaluate", new
                {
                    id = tweet.Id,
                    text = tweet.Text,
                    trend = "altjob"
                });
                var response = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {response}");
            }
        }
    }
}
