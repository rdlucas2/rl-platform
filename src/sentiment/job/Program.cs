using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tweetinvi;

namespace job
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    TwitterSettings twitterSettings = configuration.GetSection("Twitter").Get<TwitterSettings>();

                    var twitterClient = new TwitterClient(twitterSettings.ConsumerKey, twitterSettings.ConsumerSecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);
                    twitterClient.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                    services.AddHostedService(serviceProvider =>
                        new TwitterWorker(
                            serviceProvider.GetService<ILogger<TwitterWorker>>(),
                            twitterClient
                        )
                    );
                });
    }
}
