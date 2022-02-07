using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper logger;

        public TestOutputLogger(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            logger.WriteLine(formatter(state, exception));
        }
    }
}
