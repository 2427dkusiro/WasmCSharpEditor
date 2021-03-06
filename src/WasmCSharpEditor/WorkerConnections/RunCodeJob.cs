using CSharpCompiler;

namespace WasmCSharpEditor.WorkerConnections;

/// <summary>
/// コード実行ジョブを表現します。
/// </summary>
public class RunCodeJob
{
    /// <summary>
    /// <see cref="RunCodeJob"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    public RunCodeJob() { }

    /// <summary>
    /// 実行するアセンブリのIDを取得または設定します。
    /// </summary>
    public byte[] Assembly { get; set; }

    /// <summary>
    /// 標準出力への書き込みが要求されたときに実行される関数を取得または設定します。
    /// </summary>
    public Action<string?>? WriteStdOutCallBack { get; set; }

    /// <summary>
    /// 標準エラー出力への書き込みが要求されたときに実行される関数を取得または設定します。
    /// </summary>
    public Action<string?>? WriteStdErrorCallBack { get; set; }

    /// <summary>
    /// 標準入力の一文字分読み取りが要求されたときに実行される関数を取得または設定します。
    /// </summary>
    public Action<string>? StdInputReadCallBack { get; set; }

    /// <summary>
    /// 標準入力の一行分読み取りが要求されたときに実行される関数を取得または設定します。
    /// </summary>
    public Action<string>? StdInputReadLineCallBack { get; set; }

    /// <summary>
    /// コードの実行結果を取得または設定します。
    /// </summary>
    public RunCodeResult? RunCodeResult { get; set; }

    /// <summary>
    /// コード実行の進捗を取得または設定します。
    /// </summary>
    public RunCodeStatus? RunCodeStatus { get; set; }
}

/// <summary>
/// コード実行の進捗を表現します。
/// </summary>
public enum RunCodeStatus
{
    /// <summary>
    /// 実行待機中。
    /// </summary>
    RunWaiting,

    /// <summary>
    /// 実行中。
    /// </summary>
    Running,

    /// <summary>
    /// 実行完了。
    /// </summary>
    Completed,

    /// <summary>
    /// 未定義。
    /// </summary>
    Default,
}