using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;

using Telegram.Microsoft.Extensions.Logging.Internal.Services;

namespace Telegram.Microsoft.Extensions.Logging
{
    [ProviderAlias("Telegram")]
    public class TelegramLoggerProvider : ILoggerProvider
    {
        private readonly TelegramLoggerOptions _config;

        private readonly ConcurrentDictionary<string, TelegramLogger> _loggers = new ConcurrentDictionary<string, TelegramLogger>();

        private readonly ITelegramMessageService _telegramMessageService;

        public TelegramLoggerProvider(IOptionsMonitor<TelegramLoggerOptions> config, ITelegramMessageService telegramMessageService)
        {
            _config = config.CurrentValue;
            _telegramMessageService = telegramMessageService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TelegramLogger(name, _config, _telegramMessageService));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
