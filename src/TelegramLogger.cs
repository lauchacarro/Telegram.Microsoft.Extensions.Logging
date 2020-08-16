using System;
using System.Text;

using Microsoft.Extensions.Logging;

using Telegram.Microsoft.Extensions.Logging.Internal;
using Telegram.Microsoft.Extensions.Logging.Internal.Services;

namespace Telegram.Microsoft.Extensions.Logging
{
    public class TelegramLogger : ILogger
    {
        private readonly string _name;
        private readonly TelegramLoggerOptions _config;
        private readonly ITelegramMessageService _telegramMessageService;

        public TelegramLogger(string name, TelegramLoggerOptions config, ITelegramMessageService telegramMessageService)
        {
            _name = name;
            _config = config;
            _telegramMessageService = telegramMessageService;
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
                _telegramMessageService.QueueMessage(new LogMessageEntry
                {
                    ChatId = _config.ChatId,
                    Message = message.ToString()
                });

                if (logLevel >= _config.LogToStandardErrorThreshold && _config.LogErrorChatId != 0)
                {
                    _telegramMessageService.QueueMessage(new LogMessageEntry
                    {
                        ChatId = _config.LogErrorChatId,
                        Message = message.ToString()
                    });
                }
            }
            else
            {
                _telegramMessageService.SendMessage(new LogMessageEntry
                {
                    ChatId = _config.ChatId,
                    Message = message.ToString()
                }).GetAwaiter().GetResult();

                if (logLevel >= _config.LogToStandardErrorThreshold && _config.LogErrorChatId != 0)
                {
                    _telegramMessageService.SendMessage(new LogMessageEntry
                    {
                        ChatId = _config.LogErrorChatId,
                        Message = message.ToString()
                    }).GetAwaiter().GetResult();
                }
            }
        }

        private static string GetLogLevelString(LogLevel logLevel) =>

             logLevel switch
             {
                 LogLevel.Trace => "trce",
                 LogLevel.Debug => "dbug",
                 LogLevel.Information => "info",
                 LogLevel.Warning => "warn",
                 LogLevel.Error => "fail",
                 LogLevel.Critical => "crit",
                 _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
             };
    }
}
