using CodeRunner.IO;

using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly Dictionary<Guid, CompileResult> resultDictionary = new Dictionary<Guid, CompileResult>();

        private readonly string baseUrl = @"https://localhost:44339/WasmCSharpEditor/";

        /// <summary>
        /// <see cref="CodeCompileService"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        public CodeCompileService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
            cSharpCompiler = new CSharpCompiler(httpClient);
            // CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
            // ライブラリがカルチャ依存DLL読んでくれないので暫定保留。
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
            await cSharpCompiler.InitializeAsync();
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