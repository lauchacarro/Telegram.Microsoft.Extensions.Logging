using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Microsoft.Extensions.Logging.Internal;
using Telegram.Microsoft.Extensions.Logging.Internal.Services;

namespace Telegram.Microsoft.Extensions.Logging
{
    public static class TelegramLoggerExtensions
    {
        public static ILoggingBuilder AddTelegramLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TelegramLoggerProvider>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TelegramLoggerOptions>, TelegramLoggerOptionsSetup>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TelegramLoggerOptions>, LoggerProviderOptionsChangeTokenSource<TelegramLoggerOptions, TelegramLoggerProvider>>());

            var telegramLoggerOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<TelegramLoggerOptions>>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TelegramLoggerProvider>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITelegramBotClient>(new TelegramBotClient(telegramLoggerOptions.CurrentValue.BotToken)));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITelegramMessageService, TelegramMessageService>());

            builder.Services.AddHostedService<QueuedHostedService>();


            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IBackgroundLogMessageEntryQueue, BackgroundLogMessageEntryQueue>());

            return builder;
        }

        public static ILoggingBuilder AddTelegramLogger
               (this ILoggingBuilder builder, Action<TelegramLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddTelegramLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
