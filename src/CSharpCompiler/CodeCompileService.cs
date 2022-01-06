using CSharpCompiler.IO;

using JSWrapper.WorkerSyncConnection;

namespace CSharpCompiler;

/// <summary>
/// アプリケーションがユーザーコードを分離環境で実行するための、疎結合インターフェイスを提供します。
/// </summary>
public static class CodeCompileService
{
    private static CSharpCompiler cSharpCompiler;

    /// <summary>
    /// コンパイラが初期化済みかどうかを表す値を取得します。
    /// </summary>
    public static bool IsCompilerInitialized => cSharpCompiler.IsInitialized;

    /// <summary>
    /// コンパイラのバージョンを表現する、カルチャ依存の文字列を取得します。
    /// </summary>
    public static string? CompilerVersionString => cSharpCompiler.VersionString;

    /// <summary>
    /// コンパイラを初期化します。
    /// </summary>
    /// <returns></returns>
    public static async Task InitializeCompilerAsync(string basePath)
    {
        BlazorTask.WorkerContext.HttpClient.BaseAddress = new Uri(basePath);
        cSharpCompiler = new CSharpCompiler(BlazorTask.WorkerContext.HttpClient);
        await cSharpCompiler.InitializeAsync();
    }

    /// <summary>
    /// コードをコンパイルします。
    /// </summary>
    /// <param name="code">コンパイルする C# コード。</param>
    /// <returns></returns>
    public static async Task<CompilerResultMessage> CompileAsync(string code)
    {
        CompileResult result = await cSharpCompiler.CompileAsync(code);
        return CompilerResultMessage.FromCompileResult(result, result.Assembly);
    }

    // 実行

    /// <summary>
    /// 標準出力への書き込みが要求されたときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// このイベントをリッスンし、ユーザーの書き込んだ文字列を表示する処理を実装します。
    /// </remarks>
    public static Action<string>? StdOutWriteRequested;

    /// <summary>
    /// 標準エラー出力への書き込みが要求されたときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// このイベントをリッスンし、ユーザーの書き込んだ文字列を表示する処理を実装します。
    /// </remarks>
    public static Action<string>? StdErrorWriteRequested;

    /// <summary>
    /// 標準入力の１文字分の読み取りが要求されたときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// このイベントをリッスンし、イベントの発生に応じて入力の読み取りを開始します。
    /// </remarks>
    public static Action<string>? StdInputReadRequested;

    /// <summary>
    /// 標準入力の１行分の読み取りが要求されたときに発生するイベント。
    /// </summary>
    /// <remarks>
    /// このイベントをリッスンし、イベントの発生に応じて入力の読み取りを開始します。
    /// </remarks>
    public static Action<string>? StdInputReadLineRequested;

    private static WorkerConsoleReaderService? workerConsoleReaderService;

    /// <summary>
    /// コードを実行します。
    /// </summary>
    /// <param name="guid">実行するアセンブリのID。</param>
    /// <returns></returns>
    public static RunCodeResult RunCodeAsync(byte[] bin, Guid guid)
    {
        var url = BlazorTask.WorkerContext.HttpClient.BaseAddress?.AbsoluteUri ?? throw new InvalidOperationException();
        if (workerConsoleReaderService is null)
        {
            workerConsoleReaderService = new WorkerConsoleReaderService(BlazorTask.WorkerContext.WorkerJSRuntime, url);
        }

        var stdOut = new EventTextWriter((object? sender, string? str) => BlazorTask.WorkerContext.Parent.Call(typeof(CodeCompileService).GetMethod(nameof(OnWrite)), str).GetAwaiter());
        var stdError = new EventTextWriter((object? sender, string? str) => BlazorTask.WorkerContext.Parent.Call(typeof(CodeCompileService).GetMethod(nameof(OnError)), str).GetAwaiter());
        var stdIn = new WorkerTextReader((object? sender, Guid guid) => BlazorTask.WorkerContext.Parent.Call(typeof(CodeCompileService).GetMethod(nameof(OnRead)), guid.ToString()).GetAwaiter(), (object? sender, Guid guid) => BlazorTask.WorkerContext.Parent.Call(typeof(CodeCompileService).GetMethod(nameof(OnReadLine)), guid.ToString()).GetAwaiter(), workerConsoleReaderService.GetReader());

        return CodeExecuter.RunCode(bin, stdIn, stdOut, stdError);
    }

    public static void OnRead(string str)
    {
        StdInputReadRequested?.Invoke(str);
    }
    public static void OnReadLine(string str)
    {
        StdInputReadLineRequested?.Invoke(str);
    }
    public static void OnWrite(string str)
    {
        StdOutWriteRequested?.Invoke(str);
    }
    public static void OnError(string str)
    {
        StdErrorWriteRequested?.Invoke(str);
    }

}

/// <summary>
/// コード実行結果を表現します。
/// </summary>
public class RunCodeResult
{
    /// <summary>
    /// コード実行が成功したかどうか。
    /// </summary>
    public bool IsSuccessed { get; set; }

    /// <summary>
    /// 発生した例外。例外が発生しなかった場合、<c>null</c> にできます。
    /// </summary>
    public BlazorTask.Dispatch.WorkerException? OccurredException { get; set; }
}