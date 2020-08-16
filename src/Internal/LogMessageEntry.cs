namespace Telegram.Microsoft.Extensions.Logging.Internal
{
    public struct LogMessageEntry
    {
        public string Message { get; set; }
        public long ChatId { get; set; }
    }
}
