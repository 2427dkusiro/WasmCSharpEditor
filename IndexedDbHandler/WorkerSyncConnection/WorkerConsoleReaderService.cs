using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSWrapper.WorkerSyncConnection
{
    public class WorkerConsoleReaderService
    {
        private readonly IJSInProcessRuntime jsRuntime;
        public WorkerConsoleReaderService(IJSInProcessRuntime jsRuntime, string baseUrl)
        {
            this.jsRuntime = jsRuntime;
            Initialize(baseUrl);
        }

        private void Initialize(string baseUrl)
        {
            jsRuntime.InvokeVoid("importLocalScripts", SettingProvider.JSDirPath + "WorkerConsoleReader.js");
            jsRuntime.InvokeVoid("SetBaseUrl", baseUrl);
        }

        public WorkerConsoleReader GetReader()
        {
            return new WorkerConsoleReader(jsRuntime);
        }
    }
}
