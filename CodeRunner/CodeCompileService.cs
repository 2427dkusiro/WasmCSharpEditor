using CodeRunner.IO;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        private readonly Dictionary<Guid, CompileResult> resultDictionary = new Dictionary<Guid, CompileResult>();

        /// <summary>
        /// <see cref="CodeCompileService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
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
        public string CompilerVersionString => cSharpCompiler.VersionString;

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
        public async Task<string> InitializeCompilerAwaitableAsync()
        {
            await InitializeCompilerAsync();
            return null;
        }

        public async Task<string> TestJS()
        {
            var key = "001";
            await jSRuntime.InvokeVoidAsync("importLocalScripts", "./js/DbOperationsWorker.js");
            await jSRuntime.InvokeVoidAsync("Load");
            await jSRuntime.InvokeVoidAsync("Open");
            var result = await jSRuntime.InvokeAsync<string>("Read", key);
            Console.WriteLine(result);
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

        public event EventHandler<string> StdOutWrited;
        public event EventHandler<string> StdErrorWrited;

        public async Task<RunCodeResult> RunCodeAsync(Guid guid)
        {
            if (!resultDictionary.TryGetValue(guid, out var result))
            {
                return null;
            }

            var stdOut = new EventTextWriter((object sender, string str) => StdOutWrited?.Invoke(sender, str));
            var stdError = new EventTextWriter((object sender, string str) => StdErrorWrited?.Invoke(sender, str));

            var stdIn = new StreamReader(new MemoryStream()); //仮実装

            return await CodeExecuter.RunCode(result, stdIn, stdOut, stdError);
        }
    }

    public class RunCodeResult
    {
        /// <summary>
        /// コード実行が成功したかどうか。
        /// </summary>
        public bool IsSuccessed { get; set; }

        /// <summary>
        /// 発生した例外。
        /// </summary>
        public Exception OccurredException { get; set; }
    }
}