using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Telegram.Microsoft.Extensions.Logging.Internal.Services;

namespace Telegram.Microsoft.Extensions.Logging.Internal
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ITelegramMessageService _telegramMessageService;
        private readonly IBackgroundLogMessageEntryQueue _taskQueue;
        public QueuedHostedService(IBackgroundLogMessageEntryQueue taskQueue, ITelegramMessageService telegramMessageService)
        {
            _taskQueue = taskQueue;
            _telegramMessageService = telegramMessageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var entry = await _taskQueue.DequeueAsync(stoppingToken);

                await _telegramMessageService.SendMessage(entry);
            }
        }
    }
}
