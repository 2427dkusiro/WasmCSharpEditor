using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;

using CodeRunner;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
    /// <summary>
    /// コンパイルキューを提供します。
    /// </summary>
    public class CompileQueueService
    {
        /// <summary>
        /// <see cref="CompileQueueService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CompileQueueService() { }

        private WorkerFactory factory;
        private IWorker worker;
        IWorkerBackgroundService<CodeCompileService> service;

        private bool isInitialized;
        private bool isInitializing;
        private readonly object initializingSync = new();

        /// <summary>
        /// コンパイラが初期化されていなければ初期化し、そうでなければすぐに終了する <see cref="Task"/> を取得します。
        /// </summary>
        /// <param name="jSRuntime">コンパイラに渡す <see cref="IJSRuntime"/>。</param>
        /// <param name="httpClient">コンパイラに渡す <see cref="HttpClient"/>。</param>
        /// <returns></returns>
        public async Task TryInitialize(IJSRuntime jSRuntime, HttpClient httpClient)
        {
            if (isInitialized)
            {
                return;
            }
            lock (initializingSync)
            {
                if (isInitializing)
                {
                    return;
                }
                isInitializing = true;
            }

            await InitializeAsync(jSRuntime, httpClient);
            isInitialized = true;
        }

        private async Task InitializeAsync(IJSRuntime jSRuntime, HttpClient httpClient)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var names = await CodeRunner.DllLoader.DllInfoProvider.GetDllNames(httpClient);
            factory = new WorkerFactory(jSRuntime);
            worker = await factory.CreateAsync();
            service = await worker.CreateBackgroundServiceAsync<CodeCompileService>(options => options
                 .AddConventionalAssemblyOfService()
                 .AddHttpClient()
                 .AddAssemblies(names.ToArray())
             );
            Console.WriteLine($"ワーカー起動に要した時間:{stopwatch.ElapsedMilliseconds}ms");
            await service.RunAsync(obj => obj.InitializeCompilerAwaitableAsync());
            stopwatch.Stop();
            Console.WriteLine($"コンパイラ初期化に要した総時間:{stopwatch.ElapsedMilliseconds}ms");
        }

        private long currentWorkableJob;

        private object nextAssignSync = new();
        private long nextAssign;

        /// <summary>
        /// <see cref="CompileJob"/> をコンパイルキューに追加し、コンパイル終了後に完了する <see cref="Task"/> を取得します。
        /// コンパイル結果は渡された <see cref="CompileJob"/> に書き込みます。
        /// </summary>
        /// <param name="compileJob"></param>
        /// <returns></returns>
        public async Task CompileAsync(CompileJob compileJob)
        {
            long jobNum;
            lock (nextAssignSync)
            {
                jobNum = nextAssign;
                nextAssign++;
            }
            while (true)
            {
                if (!isInitialized)
                {
                    await Task.Delay(20);
                }
                else if (jobNum > currentWorkableJob)
                {
                    await Task.Delay(20);
                }
                else if (jobNum == currentWorkableJob)
                {
                    break;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            await CompileAsyncCore(compileJob);
            currentWorkableJob++;
        }

        private async Task CompileAsyncCore(CompileJob compileJob)
        {
            compileJob.CompileState = CompileStatus.Compiling;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var compileResult = await service.RunAsync(compiler => compiler.CompileAsync(compileJob.Code));
            stopwatch.Stop();
            compileJob.CompileResult = compileResult;
            compileJob.CompileState = CompileStatus.Completed;
            compileJob.CompileTime = stopwatch.Elapsed;
        }
    }
}
