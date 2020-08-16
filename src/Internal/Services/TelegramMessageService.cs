using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Telegram.Microsoft.Extensions.Logging.Internal.Services
{
    public class TelegramMessageService : ITelegramMessageService
    {
        private readonly IBackgroundLogMessageEntryQueue _entryQueue;

        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramMessageService(IBackgroundLogMessageEntryQueue entryQueue, ITelegramBotClient telegramBotClient)
        {
            _entryQueue = entryQueue;
            _telegramBotClient = telegramBotClient;
        }

        public void QueueMessage(LogMessageEntry messageEntry)
        {
            _entryQueue.QueueBackgroundWorkItem(messageEntry);
        }

        public Task SendMessage(LogMessageEntry messageEntry)
        {
            return _telegramBotClient.SendTextMessageAsync(
                       chatId: messageEntry.ChatId,
                       text: messageEntry.Message,
                       parseMode: ParseMode.Markdown
                    );
        }
    }
}
