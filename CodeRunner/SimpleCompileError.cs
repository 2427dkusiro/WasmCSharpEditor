using Microsoft.CodeAnalysis;

using System;
using System.Globalization;

namespace CodeRunner
{
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
        public SimpleCompileError(string? sourcePath, int line, int character, ErrorTypes errorType, string errorId, string errorMessage)
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
        public string? SourcePath { get; set; }

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
        public string? ErrorId { get; set; }

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public string? ErrorMessage { get; set; }

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
        public static SimpleCompileError FromDiagnostic(Diagnostic diagnostic, IFormatProvider? formatter = null)
        {
            if (diagnostic == null)
            {
                throw new ArgumentNullException(nameof(diagnostic));
            }
            CultureInfo formatProvider = CultureInfo.CurrentUICulture;
            LocationKind kind = diagnostic.Location.Kind;
            if (kind == LocationKind.SourceFile || kind - 3 <= LocationKind.SourceFile)
            {
                FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();
                FileLinePositionSpan mappedLineSpan = diagnostic.Location.GetMappedLineSpan();
                if (lineSpan.IsValid && mappedLineSpan.IsValid)
                {
                    string path;
                    string? basePath;
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

        private static string FormatSourcePath(string path, string? basePath, IFormatProvider? formatter)
        {
            return path;
        }

        private static (int line, int character) GetSourceSpan(Microsoft.CodeAnalysis.Text.LinePositionSpan span, IFormatProvider? formatter)
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
}
