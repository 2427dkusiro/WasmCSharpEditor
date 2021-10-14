using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSWrapper.WorkerConsoleConnection
{
    public class WorkerConsoleReader
    {
        private readonly IJSInProcessRuntime runtime;

        public WorkerConsoleReader(IJSInProcessRuntime runtime)
        {
            this.runtime = runtime;
        }

        public async Task InitializeAsync(string baseUrl)
        {
            await runtime.InvokeAsync<IJSInProcessObjectReference>("importLocalScripts", SettingProvider.JSDirPath + "WorkerConsoleReader.js");
            await runtime.InvokeVoidAsync("SetBaseUrl", baseUrl);
        }

        public void ReadInput()
        {
            string resStr = runtime.Invoke<string>("GetInput");
            XhrResult result = JsonSerializer.Deserialize<XhrResult>(resStr);
            Console.WriteLine($"XHR Result:{result}");
        }
    }

    public class XhrResult
    {
        public int Status { get; set; }

        public string Response { get; set; }

        public override string ToString()
        {
            return $"{Status.ToString()} : {Response}";
        }
    }
}
