using System;

using Microsoft.Extensions.Logging.Telegram.Internal;

using Telegram.Bot;

namespace Microsoft.Extensions.Logging.Telegram
{
    public class TelegramLogger : ILogger
    {
        private readonly string _name;
        private readonly TelegramLoggerOptions _config;
        private readonly IBackgroundLogMessageEntryQueue _taskQueue;

        public TelegramLogger(string name, TelegramLoggerOptions config, IBackgroundLogMessageEntryQueue taskQueue)
        {
            _name = name;
            _config = config;
            _taskQueue = taskQueue;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            bool result = logLevel != LogLevel.None
            && _config.LogLevel != LogLevel.None
            && Convert.ToInt32(logLevel) >= Convert.ToInt32(_config.LogLevel);

            return result;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }


            var message = formatter(state, exception);

            var log = $"{GetLogLevelString(logLevel)} - {DateTime.Now.ToString(_config.TimeStampFormat)} - {eventId.Id} - {_name} - {message}";

            if (exception != null)
            {
                log = $"{log} - {exception}";
            }

            if (_config.Async)
            {
                _taskQueue.QueueBackgroundWorkItem(new LogMessageEntry
                {
                    BotToken = _config.BotToken,
                    ChatId = _config.ChatId,
                    LogAsError = logLevel >= LogLevel.Error,
                    Message = log
                });
            }
            else
            {
                TelegramBotClient bot = new TelegramBotClient(_config.BotToken);
                bot.SendTextMessageAsync(
                   chatId: _config.ChatId,
                   text: log
               ).GetAwaiter().GetResult();
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

    }
}
