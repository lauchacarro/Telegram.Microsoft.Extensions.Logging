namespace Microsoft.Extensions.Logging.Telegram.Internal
{
    public struct LogMessageEntry
    {
        public string LevelString;
        public string Message;
        public bool LogAsError;
        public string BotToken;
        public long ChatId;
    }
}
