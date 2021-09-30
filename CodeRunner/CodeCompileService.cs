using Microsoft.CodeAnalysis;

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
    }

    /// <summary>
    /// 軽量でシリアル化が容易なコンパイル結果を表現します。
    /// </summary>
    /// <remarks>
    /// コンパイルされたアセンブリの参照の代わりに、Guid を使用します。
    /// </remarks>
    public class CompilerResultMessage
    {
        public CompilerResultMessage() { }

        /// <summary>
        /// <see cref="CompilerResultMessage"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="isSuccessed">コンパイルが成功したかどうか。</param>
        /// <param name="hasMainMethod">メインメソッドが正しく検出されたかどうか。</param>
        /// <param name="diagnostics">コンパイルからのメッセージ。</param>
        /// <param name="compileId">コンパイルに一意に関連付けられたID。</param>
        public CompilerResultMessage(bool isSuccessed, bool hasMainMethod, IEnumerable<SimpleCompileError> diagnostics, Guid compileId)
        {
            IsSuccessed = isSuccessed;
            HasMainMethod = hasMainMethod;
            Diagnostics = diagnostics;
            CompileId = compileId;
        }

        /// <summary>
        /// <see cref="CompileResult"/> クラスを元に、<see cref="CompilerResultMessage"/> クラスの新しいインスタンスを取得します。
        /// </summary>
        /// <param name="compileResult"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static CompilerResultMessage FromCompileResult(CompileResult compileResult, Guid guid)
        {
            return new CompilerResultMessage(compileResult.IsSuccessed, compileResult.MainMethod is not null, compileResult.Diagnostics.Select(d => SimpleCompileError.FromDiagnostic(d)).ToArray(), guid);
        }

        /// <summary>
        /// コンパイルが成功したかどうかを表す値を取得します。
        /// </summary>
        public bool IsSuccessed { get; set; }

        /// <summary>
        /// ただ 1 つの Main 関数を持っていたかどうかを取得します。
        /// </summary>
        public bool HasMainMethod { get; set; }

        /// <summary>
        /// コンパイラからのメッセージを取得します。
        /// </summary>
        public IEnumerable<SimpleCompileError> Diagnostics { get; set; }

        /// <summary>
        /// このコンパイルに関連付けられたIDを取得します。
        /// </summary>
        public Guid CompileId { get; set; }
    }

    /// <summary>
    /// 軽量でシリアル化が容易な、コンパイルエラーを表現します。
    /// </summary>
    public class SimpleCompileError
    {
        /// <summary>
        /// <see cref="SimpleCompileError"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public SimpleCompileError() { }

        /// <summary>
        /// <see cref="SimpleCompileError"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="sourcePath">コンパイルしたソースコードのパス。</param>
        /// <param name="line">メッセージと関連するコード行。</param>
        /// <param name="character">メッセージと関連するコード列。</param>
        /// <param name="errorType">メッセージの種類。</param>
        /// <param name="errorId">メッセージのID。CSXXXXなど。</param>
        /// <param name="errorMessage">メッセージの内容。</param>
        public SimpleCompileError(string sourcePath, int line, int character, ErrorTypes errorType, string errorId, string errorMessage)
        {
            SourcePath = sourcePath;
            Line = line;
            Character = character;
            ErrorType = errorType;
            ErrorId = errorId;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// コンパイルしたソースコードのパスを取得または設定します。
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// このメッセージに関連付けられたソースコードの行を取得または設定します。
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// このメッセージに関連付けられたソースコードの列を取得または設定します。
        /// </summary>
        public int Character { get; set; }

        /// <summary>
        /// このメッセージの種類を取得または設定します。
        /// </summary>
        public ErrorTypes ErrorType { get; set; }

        /// <summary>
        /// このメッセージのIDを取得または設定します。
        /// </summary>
        /// <example>CSXXXXなど。</example>
        public string ErrorId { get; set; }

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (SourcePath is not null || Line != 0)
            {
                return $"{SourcePath}({Line},{Character}): {ErrorType.ToString()} {ErrorId}: {ErrorMessage}";
            }
            else
            {
                return $"{ErrorType.ToString()} {ErrorId}: {ErrorMessage}";
            }
        }

        #region コード参照元:Microsoft.CodeAnalysis.DiagnosticFormatter(Microsoft.CodeAnalysis, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35)
        /// <summary>
        /// <see cref="Diagnostic"/> をもとに <see cref="SimpleCompileError"/> クラスの新しいインスタンスを初期化して取得します。
        /// </summary>
        /// <param name="diagnostic"></param>
        /// <param name="formatter">この引数は現在使用されていません。</param>
        /// <returns></returns>
        /// <remarks>
        /// このコードは、<see cref="Diagnostic.ToString"/> の実装を引用しています。
        /// </remarks>
        public static SimpleCompileError FromDiagnostic(Diagnostic diagnostic, IFormatProvider formatter = null)
        {
            if (diagnostic == null)
            {
                throw new ArgumentNullException(nameof(diagnostic));
            }
            var formatProvider = CultureInfo.GetCultureInfo("ja");
            LocationKind kind = diagnostic.Location.Kind;
            if (kind == LocationKind.SourceFile || kind - 3 <= LocationKind.SourceFile)
            {
                FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();
                FileLinePositionSpan mappedLineSpan = diagnostic.Location.GetMappedLineSpan();
                if (lineSpan.IsValid && mappedLineSpan.IsValid)
                {
                    string path;
                    string basePath;
                    if (mappedLineSpan.HasMappedPath)
                    {
                        path = mappedLineSpan.Path;
                        basePath = lineSpan.Path;
                    }
                    else
                    {
                        path = lineSpan.Path;
                        basePath = null;
                    }

                    string path2 = FormatSourcePath(path, basePath, formatter);
                    (int line, int character) tuple1 = GetSourceSpan(mappedLineSpan.Span, formatter);
                    (ErrorTypes errorType, string errorId) tuple2 = GetMessageAndPrefix(diagnostic);
                    string message = diagnostic.GetMessage(formatProvider);
                    return new SimpleCompileError(path2, tuple1.line, tuple1.character, tuple2.errorType, tuple2.errorId, message);
                }
            }
            (ErrorTypes errorType, string errorId) _tuple2 = GetMessageAndPrefix(diagnostic);
            string _message = diagnostic.GetMessage(formatProvider);
            return new SimpleCompileError(null, default, default, _tuple2.errorType, _tuple2.errorId, _message);
        }

        private static string FormatSourcePath(string path, string basePath, IFormatProvider formatter)
        {
            return path;
        }

        private static (int line, int character) GetSourceSpan(Microsoft.CodeAnalysis.Text.LinePositionSpan span, IFormatProvider formatter)
        {
            return (span.Start.Line + 1, span.Start.Character + 1);
        }

        private static (ErrorTypes errorType, string errorId) GetMessageAndPrefix(Diagnostic diagnostic)
        {
            return (diagnostic.Severity switch
            {
                DiagnosticSeverity.Hidden => ErrorTypes.Hidden,
                DiagnosticSeverity.Info => ErrorTypes.Info,
                DiagnosticSeverity.Warning => ErrorTypes.Warning,
                DiagnosticSeverity.Error => ErrorTypes.Error,
                _ => throw new NotSupportedException(),
            }, diagnostic.Id);
        }
        #endregion
    }

    /// <summary>
    /// コンパイラメッセージの種類を表現します。
    /// </summary>
    public enum ErrorTypes
    {
        /// <summary>
        /// Hidden(非表示)レベル。
        /// </summary>
        Hidden,
        /// <summary>
        /// Info(メッセージ)レベル。
        /// </summary>
        Info,
        /// <summary>
        /// Warning(警告)レベル。
        /// </summary>
        Warning,
        /// <summary>
        /// Error(エラー)レベル。
        /// </summary>
        Error,
    }
}
