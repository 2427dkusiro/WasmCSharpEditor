using Microsoft.JSInterop;

namespace JSWrapper.IndexedDb
{
    /// <summary>
    /// 標準の <see cref="IJSRuntime"/> を介して、IndexedDB を非同期で操作する <see cref="IDbAccesser"/> の実装を提供します。
    /// </summary>
    internal class DefaultDbAccesser : IDbAccesser
    {
        private readonly IJSRuntime jSRuntime;
        private IJSObjectReference module;
        private IJSObjectReference db;

        /// <summary>
        /// <see cref="DefaultDbAccesser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime"></param>
        public DefaultDbAccesser(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime ?? throw new ArgumentNullException(nameof(jSRuntime));
        }

        /// <inheritdoc/>
        public async Task Open()
        {
            module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", SettingProvider.JSDirPath + "DbOperationsNet5.js");
            db = await module.InvokeAsync<IJSObjectReference>("Open");
        }

        /// <inheritdoc />
        public async Task Create(string key, string type, byte[] value)
        {
            await module.InvokeVoidAsync("Create", db, key, type, value);
        }

        /// <inheritdoc />
        public async Task Update(string key, string type, byte[] value)
        {
            await module.InvokeVoidAsync("Update", db, key, type, value);
        }

        /// <inheritdoc />
        public async Task Put(string key, string type, byte[] value)
        {
            await module.InvokeVoidAsync("Put", db, key, type, value);
        }

        /// <inheritdoc />
        public async Task<byte[]> Read(string key)
        {
            var data = await module.InvokeAsync<string>("Read", db, key);
            return Convert.FromBase64String(data);
        }
    }
}
