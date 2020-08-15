namespace Microsoft.Extensions.Logging.Telegram
{
    public class TelegramLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public int EventId { get; set; }
        public long ChatId { get; set; }
        public string BotToken { get; set; }
        public string TimeStampFormat { get; set; } = "yyyy/MM/dd - HH:mm:ss";
        public bool Async { get; set; }

    }
}
