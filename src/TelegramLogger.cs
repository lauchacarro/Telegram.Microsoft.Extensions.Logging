using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Telegram.Microsoft.Extensions.Logging.Helpers;
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

            var variableValues = new Dictionary<string, object>
            {
                ["LogLevel"] = logLevel,
                ["LogLevelSmall"] = GetLogLevelString(logLevel),
                ["DateTime"] = DateTime.Now.ToString(_config.TimeStampFormat),
                ["EventId"] = eventId.Id,
                ["Name"] = _name,
                ["State"] = formatter(state, exception),
                ["Exception"] = exception?.ToString() ?? string.Empty
            };

            string message = StringTemplate.Render(
                logLevel < _config.LogToStandardErrorThreshold ? _config.LogFormat : _config.LogErrorFormat,
                variableValues);


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
