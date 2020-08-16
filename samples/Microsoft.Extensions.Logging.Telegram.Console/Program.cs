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
             .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
             .AddJsonFile($"appsettings.Development.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder.Build();


            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddLogging((logging) =>
                {
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                    logging.AddTelegramLogger();
                })
                .BuildServiceProvider();

            #endregion

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();



            for (int i = 0; i < 30; i++)
            {
                logger.LogInformation($"Test {i}");
            }

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
