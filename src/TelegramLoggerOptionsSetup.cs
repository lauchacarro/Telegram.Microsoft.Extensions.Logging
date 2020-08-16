using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Telegram.Microsoft.Extensions.Logging
{
    public class TelegramLoggerOptionsSetup : ConfigureFromConfigurationOptions<TelegramLoggerOptions>
    {
        public TelegramLoggerOptionsSetup(ILoggerProviderConfiguration<TelegramLoggerProvider>
                                      providerConfiguration)
            : base(providerConfiguration.Configuration)
        {
        }
    }
}
