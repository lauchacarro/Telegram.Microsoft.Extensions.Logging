using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Telegram.Internal
{
    public class LogMessageEntry
    {
        public string LevelString;
        public string Message;
        public bool LogAsError;
        public string BotToken;
        public long ChatId;
    }
}
