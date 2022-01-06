using Microsoft.JSInterop;

namespace JSWrapper.IndexedDb
{
    /// <summary>
    /// 非同期的に変数を読み書きできる、半永続的なストレージへのアクセスを簡略化します。
    /// </summary>
    public class VariableStorageService
    {
        private VariableStorageService() { }

        private IDbAccesser dbAccesser;

        /// <summary>
        /// <see cref="VariableStorageService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime">デフォルトの <see cref="IJSRuntime"/>。</param>
        /// <returns></returns>
        public static VariableStorageService CreateInstance(IJSRuntime jSRuntime)
        {
            var variableStorageService = new VariableStorageService
            {
                dbAccesser = new DefaultDbAccesser(jSRuntime)
            };
            return variableStorageService;
        }

        /// <summary>
        /// ワーカーの <see cref="IJSRuntime"/> から <see cref="VariableStorageService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime">ワーカーの <see cref="IJSRuntime"/>。</param>
        /// <returns></returns>
        public static VariableStorageService CreateInstanceFromWorker(IJSRuntime jSRuntime)
        {
            var variableStorageService = new VariableStorageService
            {
                dbAccesser = new WorkerDbAccesser(jSRuntime)
            };
            return variableStorageService;
        }

        /// <summary>
        /// ストレージから指定の型として変数を開きます。
        /// </summary>
        /// <typeparam name="T">変数の型。</typeparam>
        /// <param name="name">変数の名前。</param>
        /// <returns></returns>
        public async Task<VariableStorageAccesser<T>> OpenAsync<T>(string name)
        {
            return await VariableStorageAccesser<T>.OpenAsync(dbAccesser, name);
        }
    }
}
