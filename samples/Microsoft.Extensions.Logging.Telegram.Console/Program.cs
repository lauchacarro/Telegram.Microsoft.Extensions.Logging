using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging.Telegram.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configurations

            IConfigurationBuilder builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder.Build();


            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddLogging(x => x.AddTelegramLogger(o =>
                {
                    o.BotToken = configuration["Logging:Telegram:BotToken"];
                    o.ChatId = Convert.ToInt64(configuration["Logging:Telegram:ChatId"]);
                    o.TimeStampFormat = configuration["Logging:Telegram:TimeStampFormat"];
                }))
                .BuildServiceProvider();

            #endregion

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Test");

            try
            {
                throw new Exception("Throw Error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Internal Server Error.");
            }
        }
    }
}
