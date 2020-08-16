using Microsoft.Extensions.Logging;

namespace Telegram.Microsoft.Extensions.Logging
{
    public class TelegramLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
        public long ChatId { get; set; }
        public string BotToken { get; set; }
        public string TimeStampFormat { get; set; } = "yyyy/MM/dd - HH:mm:ss";
        public bool Async { get; set; }
        public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.Error;
        public bool BoldErrorLog { get; set; }
        public long LogErrorChatId { get; set; }
    }
}
