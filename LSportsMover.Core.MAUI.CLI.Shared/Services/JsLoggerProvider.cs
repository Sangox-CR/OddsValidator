using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSportsMover.Core.MAUI.CLI.Shared.Services
{
    public class JsLoggerProvider : ILoggerProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ConcurrentDictionary<string, JsLogger> _loggers = new();

        public JsLoggerProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new JsLogger(_jsRuntime));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    public class JsLogger : ILogger
    {
        private readonly IJSRuntime _jsRuntime;

        public JsLogger(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true; // Puedes filtrar por nivel aquí

        public async void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);

            // Llamar a la función JS que tienes en tu .js
            await _jsRuntime.InvokeVoidAsync("PrintToTerminal", message);
        }
    }
}
