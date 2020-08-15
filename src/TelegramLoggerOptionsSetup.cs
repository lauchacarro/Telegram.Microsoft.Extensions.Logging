using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Telegram
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
