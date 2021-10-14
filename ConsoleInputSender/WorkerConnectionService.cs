using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerUIConnection
{
    public class WorkerConnectionService
    {
        private readonly IJSInProcessRuntime runtime;
        private IJSInProcessObjectReference module;

        public WorkerConnectionService(IJSInProcessRuntime runtime)
        {
            this.runtime = runtime;
        }

        public async Task Initialize()
        {
            module = await runtime.InvokeAsync<IJSInProcessObjectReference>("import", "./_content/WorkerUIConnection/js/WorkerConsole.js");
        }
    }
}
