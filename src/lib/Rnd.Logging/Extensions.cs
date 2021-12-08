using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Rnd.Logging
{
    public static class ColorConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddColorConsoleLogger(
            this ILoggingBuilder builder) =>
            builder.AddColorConsoleLogger(
                new ColorConsoleLoggerConfiguration());

        public static ILoggingBuilder AddColorConsoleLogger(
            this ILoggingBuilder builder,
            Action<ColorConsoleLoggerConfiguration> configure)
        {
            var config = new ColorConsoleLoggerConfiguration();
            configure(config);

            return builder.AddColorConsoleLogger(config);
        }

        public static ILoggingBuilder AddColorConsoleLogger(
            this ILoggingBuilder builder,
            ColorConsoleLoggerConfiguration config)
        {
            builder.AddProvider(new ColorConsoleLoggerProvider(config));
            return builder;
        }

        public static ILoggingBuilder AddColorConsoleLogger
        (
            this ILoggingBuilder builder,
            LogLevel level,
            System.Drawing.Color Color
        ) => AddColorConsoleLogger(builder, new ColorConsoleLoggerConfiguration(level, Color));

    }
}
