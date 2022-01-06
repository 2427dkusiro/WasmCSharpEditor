using BlazorTask;

using CSharpCompiler;

using System.Diagnostics;

namespace WasmCSharpEditor.WorkerConnections;

/// <summary>
/// コンパイルキューを提供します。
/// </summary>
public class CompileQueueService
{
    /// <summary>
    /// <see cref="CompileQueueService"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    public CompileQueueService()
    {
    }

    public Worker Worker { get; set; }

    private bool isInitialized;
    private bool isInitializing;

    /// <summary>
    /// コンパイラが初期化されていなければ初期化し、そうでなければすぐに終了する <see cref="Task"/> を取得します。
    /// </summary>
    /// <returns></returns>
    public async Task TryInitialize(string basePath)
    {
        if (isInitialized || isInitializing)
        {
            return;
        }
        isInitializing = true;

        await InitializeAsync(basePath);
        isInitialized = true;
    }

    private async Task InitializeAsync(string basePath)
    {
        var stopwatch = Stopwatch.StartNew();

        System.Reflection.MethodInfo? init = typeof(CodeCompileService).GetMethod(nameof(CodeCompileService.InitializeCompilerAsync));
        await Worker.Call(init, basePath);

        CodeCompileService.StdInputReadRequested = (msg) => OnStdInputReadRequested(msg);
        CodeCompileService.StdInputReadLineRequested = (msg) => OnStdInputReadLineRequested(msg);
        CodeCompileService.StdOutWriteRequested = (msg) => OnStdOutReceived(msg);
        CodeCompileService.StdErrorWriteRequested = (msg) => OnStdErrorReceived(msg);

        stopwatch.Stop();
        Console.WriteLine($"コンパイラ初期化に要した総時間:{stopwatch.ElapsedMilliseconds}ms");
    }

    private long currentWorkableJob = 0;

    private readonly object nextAssignSync = new();
    private long nextAssign = 0;

    /// <summary>
    /// <see cref="CompileJob"/> をコンパイルキューに追加し、コンパイル終了後に完了する <see cref="Task"/> を取得します。
    /// コンパイル結果は渡された <see cref="CompileJob"/> に書き込みます。
    /// </summary>
    /// <param name="compileJob"></param>
    /// <param name="updateCallBack">進捗が変化したことを通知する関数。</param>
    /// <returns></returns>
    public async Task CompileAsync(CompileJob compileJob, IEnumerable<Func<Task>>? updateCallBack = null)
    {
        await Enqueue(() => CompileAsyncCore(compileJob, updateCallBack));
    }

    private async Task CompileAsyncCore(CompileJob compileJob, IEnumerable<Func<Task>>? updateCallBack = null)
    {
        if (compileJob.Code is null)
        {
            throw new ArgumentException("コンパイルするコードがnullです", nameof(compileJob));
        }

        compileJob.CompileState = CompileStatus.Compiling;
        await InvokeAllAsync(updateCallBack);

        var stopwatch = Stopwatch.StartNew();
        System.Reflection.MethodInfo? compile = typeof(CodeCompileService).GetMethod(nameof(CodeCompileService.CompileAsync));
        CompilerResultMessage compileResult = await Worker.Call<CompilerResultMessage>(compile, compileJob.Code);
        stopwatch.Stop();
        compileJob.CompileResult = compileResult;
        compileJob.CompileState = CompileStatus.Completed;
        compileJob.CompileTime = stopwatch.Elapsed;
        await InvokeAllAsync(updateCallBack);
    }

    /// <summary>
    /// <see cref="RunCodeJob"/> を実行キューに追加し、実行終了後に完了する <see cref="Task"/> を取得します。
    /// 実行結果は渡された <see cref="RunCodeJob"/> に書き込みます。
    /// </summary>
    /// <param name="runCodeJob"></param>
    /// <param name="updateCallBack">進捗が変化したことを通知する関数。</param>
    /// <returns></returns>
    public async Task RunCodeAsync(RunCodeJob runCodeJob, IEnumerable<Func<Task>>? updateCallBack = null)
    {
        await Enqueue(() => RunCodeAsyncCore(runCodeJob, updateCallBack));
    }

    private RunCodeJob? runCodeJob;

    private void OnStdOutReceived(string? e)
    {
        runCodeJob?.WriteStdOutCallBack?.Invoke(e);
    }

    private void OnStdErrorReceived(string? e)
    {
        runCodeJob?.WriteStdErrorCallBack?.Invoke(e);
    }

    private void OnStdInputReadRequested(string guid)
    {
        runCodeJob?.StdInputReadCallBack?.Invoke(guid);
    }

    private void OnStdInputReadLineRequested(string guid)
    {
        runCodeJob?.StdInputReadLineCallBack?.Invoke(guid);
    }

    private async Task RunCodeAsyncCore(RunCodeJob runCodeJob, IEnumerable<Func<Task>>? updateCallBack = null)
    {
        if (Worker is null)
        {
            throw new InvalidOperationException();
        }
        runCodeJob.RunCodeStatus = RunCodeStatus.Running;
        await InvokeAllAsync(updateCallBack);
        this.runCodeJob = runCodeJob;

        System.Reflection.MethodInfo? method = typeof(CodeCompileService).GetMethod(nameof(CodeCompileService.RunCodeAsync));
        var guid = Guid.NewGuid();
        RunCodeResult? result = await Worker.Call<RunCodeResult>(method, runCodeJob.Assembly, guid);

        runCodeJob.RunCodeStatus = RunCodeStatus.Completed;
        runCodeJob.RunCodeResult = result;
        await InvokeAllAsync(updateCallBack);
    }

    private static async Task InvokeAllAsync(IEnumerable<Func<Task>>? funcs)
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