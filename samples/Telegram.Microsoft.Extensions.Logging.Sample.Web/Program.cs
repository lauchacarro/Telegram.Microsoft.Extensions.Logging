using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Telegram.Microsoft.Extensions.Logging.Sample.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(configureLogging =>
            {
                configureLogging.AddTelegramLogger();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
