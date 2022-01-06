namespace CSharpCompiler;

/// <summary>
/// 軽量でシリアル化が容易なコンパイル結果を表現します。
/// </summary>
/// <remarks>
/// コンパイルされたアセンブリの参照の代わりに、Guid を使用します。
/// </remarks>
public class CompilerResultMessage
{
    /// <summary>
    /// <see cref="CompilerResultMessage"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    public CompilerResultMessage() { }

    /// <summary>
    /// <see cref="CompilerResultMessage"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="isSuccessed">コンパイルが成功したかどうか。</param>
    /// <param name="hasMainMethod">メインメソッドが正しく検出されたかどうか。</param>
    /// <param name="diagnostics">コンパイルからのメッセージ。</param>
    public CompilerResultMessage(bool isSuccessed, bool hasMainMethod, IEnumerable<SimpleCompileError>? diagnostics, byte[] bin)
    {
        IsSuccessed = isSuccessed;
        HasMainMethod = hasMainMethod;
        Diagnostics = diagnostics;
        Binary = bin;
    }

    /// <summary>
    /// <see cref="CompileResult"/> クラスを元に、<see cref="CompilerResultMessage"/> クラスの新しいインスタンスを取得します。
    /// </summary>
    /// <param name="compileResult"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    internal static CompilerResultMessage FromCompileResult(CompileResult compileResult, byte[] asmBin)
    {
        return new CompilerResultMessage(compileResult.IsSuccessed, compileResult.MainMethod is not null, compileResult.Diagnostics?.Select(d => SimpleCompileError.FromDiagnostic(d))?.ToArray(), asmBin);
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
    public IEnumerable<SimpleCompileError>? Diagnostics { get; set; }

    public byte[] Binary { get; set; }
}