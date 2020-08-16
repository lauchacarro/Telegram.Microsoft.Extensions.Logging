using System.Threading.Tasks;

namespace Telegram.Microsoft.Extensions.Logging.Internal.Services
{
    public interface ITelegramMessageService
    {
        Task SendMessage(LogMessageEntry messageEntry);
        void QueueMessage(LogMessageEntry messageEntry);
    }
}
