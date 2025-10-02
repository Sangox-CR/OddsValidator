using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSportsMover.Core.MAUI.CLI.Shared.Services
{
    public class JsLoggerBridge
    {
        public static JsLoggerBridge? Instance { get; private set; }

        private readonly IJSRuntime _jsRuntime;

        public JsLoggerBridge(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Instance = this;
        }

        public async Task PrintToTerminalAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("terminal.printToTerminal", message);
        }
    }
}
