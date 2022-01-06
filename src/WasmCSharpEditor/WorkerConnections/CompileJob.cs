using CSharpCompiler;

namespace WasmCSharpEditor.WorkerConnections;

/// <summary>
/// コンパイラに渡すジョブを表現します。
/// </summary>
public class CompileJob
{
    /// <summary>
    /// <see cref="CompileJob"/> クラスの新しいインスタンスを取得します。
    /// </summary>
    public CompileJob() { }

    /// <summary>
    /// コンパイルするコードを取得または設定します。
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// コンパイルの経過を取得または設定します。
    /// </summary>
    public CompileStatus CompileState { get; set; } = CompileStatus.CompileWaiting;

    /// <summary>
    /// コンパイルの所要時間を取得または設定します。
    /// </summary>
    public TimeSpan CompileTime { get; set; }

    /// <summary>
    /// コンパイルの結果を取得または設定します。
    /// </summary>
    public CompilerResultMessage? CompileResult { get; set; }
}

/// <summary>
/// コンパイルの経過を表現します。
/// </summary>
public enum CompileStatus
{
    /// <summary>
    /// コンパイル待機中。
    /// </summary>
    CompileWaiting,

    /// <summary>
    /// コンパイル中。
    /// </summary>
    Compiling,

    /// <summary>
    /// コンパイル終了済み。
    /// </summary>
    Completed,

    /// <summary>
    /// 未設定。
    /// </summary>
    Default,
}