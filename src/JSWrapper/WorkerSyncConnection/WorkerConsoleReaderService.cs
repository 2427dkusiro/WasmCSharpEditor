using Microsoft.JSInterop;

namespace JSWrapper.WorkerSyncConnection
{
    public class WorkerConsoleReaderService
    {
        private readonly IJSUnmarshalledRuntime jsRuntime;
        public WorkerConsoleReaderService(IJSUnmarshalledRuntime jsRuntime, string baseUrl)
        {
            this.jsRuntime = jsRuntime;
            Initialize(baseUrl);
        }

        private void Initialize(string baseUrl)
        {
            var msg1 = jsRuntime.InvokeUnmarshalled<string?, string>("importScripts", "../../" + SettingProvider.JSDirPath + "WorkerConsoleReader.js");
            var msg2 = jsRuntime.InvokeUnmarshalled<string?, string>("SetBaseUrl", baseUrl);
        }

        public WorkerConsoleReader GetReader()
        {
            return new WorkerConsoleReader(jsRuntime);
        }
    }
}
