using Microsoft.JSInterop;

using System.Text.Json;

namespace JSWrapper.WorkerSyncConnection
{
    public class WorkerConsoleReader
    {
        private readonly IJSUnmarshalledRuntime runtime;

        internal WorkerConsoleReader(IJSUnmarshalledRuntime runtime)
        {
            this.runtime = runtime;
        }

        private const string sig = "<!--6MENWdyDt0p4Qnp9IGYL4OSYj2/Ns9k6uv8yONpN2ph2zNKm+ILRdnvkvl9H7dqFQB+K7aXXDTXo057dUH5vKg==-->";
        public string ReadInput(string guid)
        {
            var rawResult = runtime.InvokeUnmarshalled<string?, string>("GetInput", guid);
            XhrResult result = JsonSerializer.Deserialize<XhrResult>(rawResult);
            if (result.Status == 200)
            {
                if (!result.Response.Contains(sig))
                {
                    return result.Response;
                }
                throw new InvalidOperationException("service workerによる書き換えが失敗しました。");
            }
            throw new InvalidOperationException("コンソール入力受け取りのステータスコードが200以外でした。");
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
