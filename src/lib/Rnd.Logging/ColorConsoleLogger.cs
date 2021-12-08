using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Pastel;

namespace Rnd.Logging
{
    public class ColorConsoleLoggerConfiguration
    {
        public int EventId { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.Green;

        public ColorConsoleLoggerConfiguration()
        {
        }

        public ColorConsoleLoggerConfiguration(int eventId, LogLevel logLevel, System.Drawing.Color color)
        {
            EventId = eventId;
            LogLevel = logLevel;
            Color = color;
        }

        public ColorConsoleLoggerConfiguration(LogLevel logLevel, System.Drawing.Color color)
        {
            LogLevel = logLevel;
            Color = color;
        }
    }

    public class ColorConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ColorConsoleLoggerConfiguration _config;

        
        
        public ColorConsoleLogger
        (
            string name,
            ColorConsoleLoggerConfiguration config
        )
        {
            // TODO: Add excluded options

            //DisplayName = name.StartsWith("FourServerProxy") ? name.Split('.').LastOrDefault() : name;

            DisplayName = name.Split('.').LastOrDefault();
            (_name, _config) = (name, config);
        }

        public string? DisplayName { get; set; }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel == _config.LogLevel;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_config.EventId != 0 && _config.EventId != eventId.Id)
            {
                return;
            }

            var sb = new StringBuilder()
                .Append($"[{logLevel}] ".Pastel(_config.Color))
                .Append($"[{DisplayName}] - {formatter(state, exception)}");

            Console.WriteLine(sb.ToString());
        }
    }

    public sealed class ColorConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ColorConsoleLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, ColorConsoleLogger> _loggers = new();

        public ColorConsoleLoggerProvider(ColorConsoleLoggerConfiguration config) =>
            _config = config;

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new ColorConsoleLogger(name, _config));

        public void Dispose() => _loggers.Clear();
    }

}