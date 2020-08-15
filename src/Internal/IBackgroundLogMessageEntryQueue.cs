using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.Telegram.Internal
{
    public interface IBackgroundLogMessageEntryQueue
    {
        public void QueueBackgroundWorkItem(LogMessageEntry entry);

        Task<LogMessageEntry> DequeueAsync(CancellationToken cancellationToken);
    }
}
