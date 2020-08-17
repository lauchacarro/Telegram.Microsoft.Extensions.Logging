using Microsoft.Extensions.Logging;

namespace Telegram.Microsoft.Extensions.Logging
{
    public class TelegramLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
        public long ChatId { get; set; }
        public string BotToken { get; set; }
        public string TimeStampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff \"GMT\"zzz";
        public string LogFormat { get; set; } = @"{LogLevelSmall} - {DateTime} - {EventId} - {Name} - {State}";
        public bool Async { get; set; }
        public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.Error;
        public long LogErrorChatId { get; set; }
        public string LogErrorFormat { get; set; } = @"{LogLevel} - {DateTime} - {EventId} - {Name} - {State} - {Exception}";

    }
}
