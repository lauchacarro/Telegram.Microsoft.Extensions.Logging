using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.Telegram.Internal
{
    public class BackgroundLogMessageEntryQueue : IBackgroundLogMessageEntryQueue
    {
        private readonly ConcurrentQueue<LogMessageEntry> _entries = new ConcurrentQueue<LogMessageEntry>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public BackgroundLogMessageEntryQueue()
        {
        }

        public void QueueBackgroundWorkItem(LogMessageEntry entry)
        {
            _entries.Enqueue(entry);
            _signal.Release();
        }

        public async Task<LogMessageEntry> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _entries.TryDequeue(out var entry);

            return entry;
        }
    }
}
