using CSharpCompiler;

using WasmCSharpEditor.WorkerConnections;

namespace WasmCSharpEditor.UILogics;

/// <summary>
/// コードエディタコンポーネント間で共有されるデータを表現します。
/// </summary>
public class CodeEditorContext
{
    private readonly CompileQueueService compileQueueService;

    /// <summary>
    /// <see cref="CodeEditorContext"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="compileQueueService"></param>
    public CodeEditorContext(CompileQueueService compileQueueService)
    {
        this.compileQueueService = compileQueueService;
    }

    /// <summary>
    /// コードをコンパイルし、コンパイルの経過と結果をこのオブジェクトに設定します。
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task Compile(string code)
    {
        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        var job = new CompileJob()
        {
            Code = code,
        };
        compileJob = job;
        await compileQueueService.CompileAsync(job, updateUiCallBacks);
    }

    /// <summary>
    /// コードを実行し、実行の経過と結果をこのオブジェクトに設定します。
    /// </summary>
    /// <returns></returns>
    public async Task RunCodeAsync()
    {
        if (compileJob is null || compileJob.CompileResult is null)
        {
            throw new InvalidOperationException("コンパイルが実行されていません");
        }
        if (!compileJob.CompileResult.IsSuccessed)
        {
            throw new InvalidOperationException("コンパイル失敗した結果は実行できません");
        }
        if (WriteStdOut is null || WriteStdError is null || StdInputRead is null || StdInputReadLine is null)
        {
            throw new InvalidOperationException("コードを実行する前に標準入出力リダイレクト用関数を設定する必要があります。");
        }

        var job = new RunCodeJob()
        {
            Assembly = compileJob.CompileResult.Binary,
            WriteStdOutCallBack = WriteStdOut,
            WriteStdErrorCallBack = WriteStdError,
            StdInputReadCallBack = StdInputRead,
            StdInputReadLineCallBack = StdInputReadLine,
        };
        runCodeJob = job;
        await compileQueueService.RunCodeAsync(job, updateUiCallBacks);
    }

    private CompileJob? compileJob;
    private RunCodeJob? runCodeJob;

    private readonly List<Func<Task>> updateUiCallBacks = new();

    /// <summary>
    /// UIを描画更新すべきときに実行される関数を追加します。
    /// </summary>
    /// <param name="func"></param>
    public void AddUpdateUiCallBack(Func<Task> func)
    {
        updateUiCallBacks.Add(func);
    }

    /// <summary>
    /// UIを描画更新すべきときに実行される関数を削除します。
    /// </summary>
    /// <param name="func"></param>
    public void RemoveUpdateUiCallBack(Func<Task> func)
    {
        updateUiCallBacks.Remove(func);
    }

    /// <summary>
    /// コンパイルの状態を取得します。
    /// </summary>
    public CompileStatus CompileState => compileJob?.CompileState ?? CompileStatus.Default;

    /// <summary>
    /// コンパイルの結果を取得します。
    /// </summary>
    public CompilerResultMessage? CompileResult => compileJob?.CompileResult;

    /// <summary>
    /// コンパイルにかかった時間を取得します。
    /// </summary>
    public TimeSpan? CompileTime => compileJob?.CompileTime;

    /// <summary>
    /// 標準出力の書き込みを実行する関数を取得または設定します。
    /// </summary>
    public Action<string?>? WriteStdOut { get; set; }

    /// <summary>
    /// 標準エラー出力の書き込みを実行する関数を取得または設定します。
    /// </summary>
    public Action<string?>? WriteStdError { get; set; }

    /// <summary>
    /// 標準入力から一文字読み取りを開始する関数を取得または設定します。
    /// </summary>
    public Action<string>? StdInputRead { get; set; }

    /// <summary>
    /// 標準入力から一行読み取りを開始する関数を取得または設定します。
    /// </summary>
    public Action<string>? StdInputReadLine { get; set; }

    /// <summary>
    /// コード実行の進捗を取得します。
    /// </summary>
    public RunCodeStatus RunCodeState => runCodeJob?.RunCodeStatus ?? RunCodeStatus.Default;

    /// <summary>
    /// コード実行の結果を取得します。
    /// </summary>
    public RunCodeResult? RunCodeResult => runCodeJob?.RunCodeResult;
}