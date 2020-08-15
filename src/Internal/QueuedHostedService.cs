using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Microsoft.Extensions.Logging.Telegram.Internal
{
    public class QueuedHostedService : BackgroundService
    {
        public QueuedHostedService(IBackgroundLogMessageEntryQueue taskQueue)
        {
            TaskQueue = taskQueue;
        }

        public IBackgroundLogMessageEntryQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var entry = await TaskQueue.DequeueAsync(stoppingToken);

                TelegramBotClient bot = new TelegramBotClient(entry.BotToken);
                await bot.SendTextMessageAsync(
                   chatId: entry.ChatId,
                   text: entry.Message
               );
            }
        }
    }
}
