using CodeRunner.IO;

using JSWrapper.WorkerSyncConnection;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    /// <summary>
    /// アプリケーションがユーザーコードを分離環境で実行するための、疎結合インターフェイスを提供します。
    /// </summary>
    public class CodeCompileService
    {
        private readonly CSharpCompiler cSharpCompiler;
        private readonly HttpClient httpClient;
        private readonly IJSRuntime jSRuntime;

        private readonly Dictionary<Guid, CompileResult> resultDictionary = new();

        /// <summary>
        /// <see cref="CodeCompileService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        /// <param name="jSRuntime">有効な <see cref="IJSRuntime"/>。<see cref="IJSInProcessRuntime"/>を実装することが実際には必要です。</param>
        public CodeCompileService(HttpClient httpClient, IJSRuntime jSRuntime)
        {
            this.httpClient = httpClient;
            this.jSRuntime = jSRuntime;
            cSharpCompiler = new CSharpCompiler(httpClient);
        }

        /// <summary>
        /// 親ワーカーから、baseURL と現在のカルチャーを伝播させます。
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="culture"></param>
        public void ApplyParentContext(string baseUrl, string culture)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }

        /// <summary>
        /// コンパイラが初期化済みかどうかを表す値を取得します。
        /// </summary>
        public bool IsCompilerInitialized => cSharpCompiler.IsInitialized;

        /// <summary>
        /// コンパイラのバージョンを表現する、カルチャ依存の文字列を取得します。
        /// </summary>
        public string? CompilerVersionString => cSharpCompiler.VersionString;

        /// <summary>
        /// コンパイラを初期化します。
        /// </summary>
        /// <returns></returns>
        public async Task InitializeCompilerAsync()
        {
            await cSharpCompiler.InitializeAsync();
        }

        /// <summary>
        /// コンパイラを初期化して、<c>null</c> を <see cref="string"/> として返します。
        /// </summary>
        /// <returns>常に <c>null</c> が返されます。</returns>
        /// <remarks>ライブラリの制約により、戻り値が <see cref="Task"/> 型であるメソッドを正しく待機することができないようであるため、このメソッドを利用することでコンパイラの初期化を待機することができます。</remarks>
        public async Task<string?> InitializeCompilerAwaitableAsync()
        {
            await InitializeCompilerAsync();
            return null;
        }

        /// <summary>
        /// コードをコンパイルします。
        /// </summary>
        /// <param name="code">コンパイルする C# コード。</param>
        /// <returns></returns>
        public async Task<CompilerResultMessage> CompileAsync(string code)
        {
            CompileResult result = await cSharpCompiler.CompileAsync(code);
            var guid = Guid.NewGuid();
            resultDictionary.Add(guid, result);
            return CompilerResultMessage.FromCompileResult(result, guid);
        }

        /// <summary>
        /// 標準出力への書き込みが要求されたときに発生するイベント。
        /// </summary>
        /// <remarks>
        /// このイベントをリッスンし、ユーザーの書き込んだ文字列を表示する処理を実装します。
        /// </remarks>
        public event EventHandler<string?>? StdOutWriteRequested;

        /// <summary>
        /// 標準エラー出力への書き込みが要求されたときに発生するイベント。
        /// </summary>
        /// <remarks>
        /// このイベントをリッスンし、ユーザーの書き込んだ文字列を表示する処理を実装します。
        /// </remarks>
        public event EventHandler<string?>? StdErrorWriteRequested;

        /// <summary>
        /// 標準入力の１文字分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        /// <remarks>
        /// このイベントをリッスンし、イベントの発生に応じて入力の読み取りを開始します。
        /// </remarks>
        public event EventHandler<string>? StdInputReadRequested;

        /// <summary>
        /// 標準入力の１行分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        /// <remarks>
        /// このイベントをリッスンし、イベントの発生に応じて入力の読み取りを開始します。
        /// </remarks>
        public event EventHandler<string>? StdInputReadLineRequested;


        private WorkerConsoleReaderService? workerConsoleReaderService;

        /// <summary>
        /// コードを実行します。
        /// </summary>
        /// <param name="guid">実行するアセンブリのID。</param>
        /// <returns></returns>
        public RunCodeResult RunCodeAsync(Guid guid)
        {
            if (!resultDictionary.TryGetValue(guid, out CompileResult? result))
            {
                throw new ArgumentException("指定されたアセンブリが見つかりません", nameof(guid));
            }
            var url = httpClient.BaseAddress?.AbsoluteUri ?? throw new InvalidOperationException();
            if (workerConsoleReaderService is null)
            {
                workerConsoleReaderService = new WorkerConsoleReaderService(jSRuntime as IJSInProcessRuntime, url);
            }

            var stdOut = new EventTextWriter((object? sender, string? str) => StdOutWriteRequested?.Invoke(sender, str));
            var stdError = new EventTextWriter((object? sender, string? str) => StdErrorWriteRequested?.Invoke(sender, str));
            var stdIn = new WorkerTextReader((object? sender, Guid guid) => StdInputReadRequested?.Invoke(sender, guid.ToString()), (object? sender, Guid guid) => StdInputReadLineRequested?.Invoke(sender, guid.ToString()), workerConsoleReaderService.GetReader());

            return CodeExecuter.RunCode(result, stdIn, stdOut, stdError);
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
        public Exception? OccurredException { get; set; }
    }
}