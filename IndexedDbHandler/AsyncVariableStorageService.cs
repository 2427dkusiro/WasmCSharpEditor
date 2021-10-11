using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDbHandler
{
    /// <summary>
    /// 非同期的に変数を読み書きできる、半永続的なストレージへのアクセスを簡略化します。
    /// </summary>
    public class AsyncVariableStorageService
    {
        private AsyncVariableStorageService() { }

        private IAsyncDbAccesser dbAccesser;

        /// <summary>
        /// <see cref="AsyncVariableStorageService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime">デフォルトの <see cref="IJSRuntime"/>。</param>
        /// <returns></returns>
        public static AsyncVariableStorageService CreateInstance(IJSRuntime jSRuntime)
        {
            var variableStorageService = new AsyncVariableStorageService
            {
                dbAccesser = new DefaultAsyncDbAccesser(jSRuntime)
            };
            return variableStorageService;
        }

        /// <summary>
        /// ワーカーの <see cref="IJSRuntime"/> から <see cref="AsyncVariableStorageService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="jSRuntime">ワーカーの <see cref="IJSRuntime"/>。</param>
        /// <returns></returns>
        public static AsyncVariableStorageService CreateInstanceFromWorker(IJSRuntime jSRuntime)
        {
            var variableStorageService = new AsyncVariableStorageService
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
        public async Task<VariableStorageAsyncAccesser<T>> OpenAsync<T>(string name)
        {
            return await VariableStorageAsyncAccesser<T>.OpenAsync(dbAccesser, name);
        }
    }
}
