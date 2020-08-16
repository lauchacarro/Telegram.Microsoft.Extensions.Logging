using System.Threading;
using System.Threading.Tasks;

namespace Telegram.Microsoft.Extensions.Logging.Internal
{
    public interface IBackgroundLogMessageEntryQueue
    {
        public void QueueBackgroundWorkItem(LogMessageEntry entry);

        Task<LogMessageEntry> DequeueAsync(CancellationToken cancellationToken);
    }
}
