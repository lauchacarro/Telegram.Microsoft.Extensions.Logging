using System.Collections.Concurrent;

using Microsoft.Extensions.Logging.Telegram.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Telegram
{
    [ProviderAlias("Telegram")]
    public class TelegramLoggerProvider : ILoggerProvider
    {
        private readonly TelegramLoggerOptions _config;

        private readonly ConcurrentDictionary<string, TelegramLogger> _loggers = new ConcurrentDictionary<string, TelegramLogger>();

        private readonly IBackgroundLogMessageEntryQueue _entryQueue;

        public TelegramLoggerProvider(IOptionsMonitor<TelegramLoggerOptions> config, IBackgroundLogMessageEntryQueue entryQueue)
        {
            _config = config.CurrentValue;
            _entryQueue = entryQueue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TelegramLogger(name, _config, _entryQueue));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
