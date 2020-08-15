using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Telegram.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Telegram
{
    public static class TelegramLoggerExtensions
    {
        static public ILoggingBuilder AddTelegramLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();


            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider,
            TelegramLoggerProvider>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton
               <IConfigureOptions<TelegramLoggerOptions>, TelegramLoggerOptionsSetup>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton
               <IOptionsChangeTokenSource<TelegramLoggerOptions>,
            LoggerProviderOptionsChangeTokenSource<TelegramLoggerOptions, TelegramLoggerProvider>>());

            builder.Services.AddHostedService<QueuedHostedService>();
            builder.Services.AddSingleton<IBackgroundLogMessageEntryQueue, BackgroundLogMessageEntryQueue>();
            return builder;
        }

        static public ILoggingBuilder AddTelegramLogger
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
