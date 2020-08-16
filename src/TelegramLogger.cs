using System;
using System.Text;

using Microsoft.Extensions.Logging.Telegram.Internal;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

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

            StringBuilder message = new StringBuilder();

            message.Append(GetLogLevelString(logLevel));
            message.Append("-");
            message.Append(DateTime.Now.ToString(_config.TimeStampFormat));
            message.Append("-");
            message.Append(eventId.Id);
            message.Append("-");
            message.Append(_name);
            message.Append("-");
            message.Append(formatter(state, exception));


            if (exception != null)
            {
                message.Append("-");
                message.Append(exception);
            }

            if (logLevel >= _config.LogToStandardErrorThreshold && _config.BoldErrorLog)
            {
                message.Insert(0, "***");
                message.Append("***");
            }

            if (_config.Async)
            {
                _taskQueue.QueueBackgroundWorkItem(new LogMessageEntry
                {
                    BotToken = _config.BotToken,
                    ChatId = _config.ChatId,
                    LogAsError = logLevel >= LogLevel.Error,
                    Message = message.ToString()
                });

                if (logLevel >= _config.LogToStandardErrorThreshold && _config.LogErrorChatId != 0)
                {
                    _taskQueue.QueueBackgroundWorkItem(new LogMessageEntry
                    {
                        BotToken = _config.BotToken,
                        ChatId = _config.LogErrorChatId,
                        LogAsError = logLevel >= LogLevel.Error,
                        Message = message.ToString()
                    });
                }
            }
            else
            {
                TelegramBotClient bot = new TelegramBotClient(_config.BotToken);
                bot.SendTextMessageAsync(
                   chatId: _config.ChatId,
                   parseMode: ParseMode.Markdown,
                   text: message.ToString()
               ).GetAwaiter().GetResult();

                if (logLevel >= _config.LogToStandardErrorThreshold && _config.LogErrorChatId != 0)
                {
                    bot.SendTextMessageAsync(
                       chatId: _config.LogErrorChatId,
                       text: message.ToString(),
                       parseMode: ParseMode.Markdown
                    ).GetAwaiter().GetResult();
                }
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
