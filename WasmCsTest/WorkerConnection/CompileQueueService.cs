using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using BlazorWorker.Extensions.JSRuntime;

using CodeRunner;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WasmCsTest.WorkerConnection
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
        private IWorkerBackgroundService<CodeCompileService> service;

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
            var stopwatch = Stopwatch.StartNew();
#warning "ライブラリ側修正次第、CurrentUICultureを読むようにして下さい。"
            var culture = CultureInfo.InvariantCulture;
            IEnumerable<string> names = await CodeRunner.DllLoader.DllInfoProvider.GetDllNames(httpClient, culture);
            factory = new WorkerFactory(jSRuntime);
            worker = await factory.CreateAsync();
            var _service = await worker.CreateBackgroundServiceAsync<CodeCompileServiceStartup>(options => options
                 .AddConventionalAssemblyOfService()
                 .AddHttpClient()
                 .AddBlazorWorkerJsRuntime()
                 .AddAssemblies(names.ToArray())
             );
            service = await _service.CreateBackgroundServiceAsync(startup => startup.Resolve<CodeCompileService>());
            Console.WriteLine($"ワーカー起動に要した時間:{stopwatch.ElapsedMilliseconds}ms");
            await service.RunAsync(obj => obj.ApplyParentContext(httpClient.BaseAddress.AbsoluteUri, culture.Name));

            await TestJSAsync(jSRuntime);
            await service.RunAsync(obj => obj.TestJS());

            await service.RunAsync(obj => obj.InitializeCompilerAwaitableAsync());
            await service.RegisterEventListenerAsync<string>(nameof(CodeCompileService.StdOutWrited), OnStdOutReceived);
            await service.RegisterEventListenerAsync<string>(nameof(CodeCompileService.StdErrorWrited), OnStdErrorReceived);
            stopwatch.Stop();
            Console.WriteLine($"コンパイラ初期化に要した総時間:{stopwatch.ElapsedMilliseconds}ms");
        }

        private async Task TestJSAsync(IJSRuntime jSRuntime)
        {
            var key = "001";
            var module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/DbOperations.js");
            var db = await module.InvokeAsync<IJSObjectReference>("Open");
            await module.InvokeVoidAsync("Put", db, key, "testData", (new[] { 'v', 'a', 'l', 'u', 'e' }).Select(x => (byte)x).ToArray());
        }

        private long currentWorkableJob = 0;

        private readonly object nextAssignSync = new();
        private long nextAssign = 0;

        private void OnStdOutReceived(object sender, string e)
        {
            runCodeJob?.WriteStdOutCallBack.Invoke(e);
        }

        private void OnStdErrorReceived(object sender, string e)
        {
            runCodeJob?.WriteStdErrorCallBack.Invoke(e);
        }

        /// <summary>
        /// <see cref="CompileJob"/> をコンパイルキューに追加し、コンパイル終了後に完了する <see cref="Task"/> を取得します。
        /// コンパイル結果は渡された <see cref="CompileJob"/> に書き込みます。
        /// </summary>
        /// <param name="compileJob"></param>
        /// <param name="updateCallBack">進捗が変化したことを通知する関数。</param>
        /// <returns></returns>
        public async Task CompileAsync(CompileJob compileJob, IEnumerable<Func<Task>> updateCallBack = null)
        {
            await Enqueue(() => CompileAsyncCore(compileJob, updateCallBack));
        }

        private async Task CompileAsyncCore(CompileJob compileJob, IEnumerable<Func<Task>> updateCallBack = null)
        {
            compileJob.CompileState = CompileStatus.Compiling;
            await InvokeAllAsync(updateCallBack);
            var stopwatch = Stopwatch.StartNew();
            CompilerResultMessage compileResult = await service.RunAsync(compiler => compiler.CompileAsync(compileJob.Code));
            stopwatch.Stop();
            compileJob.CompileResult = compileResult;
            compileJob.CompileState = CompileStatus.Completed;
            compileJob.CompileTime = stopwatch.Elapsed;
            compileJob.CompileResultId = compileResult.CompileId;
            await InvokeAllAsync(updateCallBack);
        }

        /// <summary>
        /// <see cref="RunCodeJob"/> を実行キューに追加し、実行終了後に完了する <see cref="Task"/> を取得します。
        /// 実行結果は渡された <see cref="RunCodeJob"/> に書き込みます。
        /// </summary>
        /// <param name="runCodeJob"></param>
        /// <param name="updateCallBack">進捗が変化したことを通知する関数。</param>
        /// <returns></returns>
        public async Task RunCodeAsync(RunCodeJob runCodeJob, IEnumerable<Func<Task>> updateCallBack = null)
        {
            await Enqueue(() => RunCodeAsyncCore(runCodeJob, updateCallBack));
        }

        private RunCodeJob runCodeJob;
        private async Task RunCodeAsyncCore(RunCodeJob runCodeJob, IEnumerable<Func<Task>> updateCallBack = null)
        {
            runCodeJob.RunCodeStatus = RunCodeStatus.Running;
            await InvokeAllAsync(updateCallBack);
            this.runCodeJob = runCodeJob;
            var result = await service.RunAsync(runner => runner.RunCodeAsync(runCodeJob.AssemblyId));
            runCodeJob.RunCodeStatus = RunCodeStatus.Completed;
            runCodeJob.RunCodeResult = result;
            await InvokeAllAsync(updateCallBack);
        }

        private static async Task InvokeAllAsync(IEnumerable<Func<Task>> funcs)
        {
            if (funcs is null)
            {
                return;
            }
            await Task.WhenAll(funcs.Select(x => x.Invoke()));
        }

        private async Task Enqueue(Func<Task> func)
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
                    await Task.Delay(25);
                }
                else if (jobNum > currentWorkableJob)
                {
                    await Task.Delay(25);
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
            await func();
            currentWorkableJob++;
        }
    }
}
