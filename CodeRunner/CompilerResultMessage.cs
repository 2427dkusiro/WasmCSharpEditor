
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeRunner
{
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
}
