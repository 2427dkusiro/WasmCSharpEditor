using Microsoft.JSInterop;

namespace JSWrapper.IndexedDb
{
    /// <summary>
    /// ワーカーの <see cref="IJSRuntime"/> を介して、IndexedDB を非同期で操作する <see cref="IDbAccesser"/> の実装を提供します。
    /// </summary>
    internal class WorkerDbAccesser : IDbAccesser
    {
        private readonly IJSRuntime jSRuntime;

        /// <summary>
        /// <see cref="WorkerDbAccesser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime"></param>
        public WorkerDbAccesser(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        /// <inheritdoc />
        public async Task Open()
        {
            await jSRuntime.InvokeVoidAsync("importLocalScripts", SettingProvider.JSDirPath + "DbOperationsWorkerNet5.js");
            await jSRuntime.InvokeVoidAsync("Load");
            await jSRuntime.InvokeVoidAsync("Open");
        }

        /// <inheritdoc />
        public async Task Create(string key, string type, byte[] value)
        {
            await jSRuntime.InvokeVoidAsync("Create", key, type, value);
        }

        /// <inheritdoc />
        public async Task Update(string key, string type, byte[] value)
        {
            await jSRuntime.InvokeVoidAsync("Update", key, type, value);
        }

        /// <inheritdoc />
        public async Task Put(string key, string type, byte[] value)
        {
            await jSRuntime.InvokeVoidAsync("Put", key, type, value);
        }

        /// <inheritdoc />
        public async Task<byte[]> Read(string key)
        {
            var data = await jSRuntime.InvokeAsync<string>("Read", key);
            return Convert.FromBase64String(data);
        }
    }
}
