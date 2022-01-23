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

            var creds = new ConsumerOnlyCredentials(twitterSettings.ConsumerKey, twitterSettings.ConsumerSecret) {
                BearerToken = twitterSettings.Bearer
            };

            var twitterClient = new TwitterClient(creds);

            var sampleStreamV2 = twitterClient.StreamsV2.CreateSampleStream();

            sampleStreamV2.TweetReceived += async (sender, args) =>
            {
                await SaveTweetByTrend(args.Tweet);
            };

            //var streamParams = new StartSampleStreamV2Parameters();
            //await sampleStreamV2.StartAsync(streamParams);

            await sampleStreamV2.StartAsync();
        }

        private static async Task SaveTweetByTrend(TweetV2 tweet)
        {
            Console.WriteLine($"Id: {tweet.Id}, Body: {tweet.Text}");
            tweet.
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
