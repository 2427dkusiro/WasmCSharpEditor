using CodeRunner;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
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

        public async Task RunCodeAsync()
        {
            if (compileJob is null)
            {
                throw new InvalidOperationException("コンパイルが実行されていません");
            }
            if (!compileJob.CompileResult.IsSuccessed)
            {
                throw new InvalidOperationException("コンパイル失敗した結果は実行できません");
            }

            var job = new RunCodeJob()
            {
                AssemblyId = compileJob.CompileResultId,
                WriteStdOutCallBack = WriteStdOut,
                WriteStdErrorCallBack = WriteStdError,
            };
            runCodeJob = job;
            await compileQueueService.RunCodeAsync(job, updateUiCallBacks);
        }

        private CompileJob compileJob;
        private RunCodeJob runCodeJob;

        private readonly List<Func<Task>> updateUiCallBacks = new List<Func<Task>>();
        public void AddUpdateUiCallBack(Func<Task> func) => updateUiCallBacks.Add(func);
        public void RemoveUpdateUiCallBack(Func<Task> func) => updateUiCallBacks.Remove(func);

        /// <summary>
        /// コンパイルの状態を取得します。
        /// </summary>
        public CompileStatus CompileState => compileJob?.CompileState ?? CompileStatus.Default;

        /// <summary>
        /// コンパイルの結果を取得します。
        /// </summary>
        public CompilerResultMessage CompileResult => compileJob?.CompileResult;

        /// <summary>
        /// コンパイルにかかった時間を取得します。
        /// </summary>
        public TimeSpan CompileTime => compileJob.CompileTime;

        public Action<string> WriteStdOut { get; set; }

        public Action<string> WriteStdError { get; set; }

        public RunCodeStatus RunCodeState => runCodeJob?.RunCodeStatus ?? RunCodeStatus.Default;

        public RunCodeResult RunCodeResult => runCodeJob?.RunCodeResult;
    }
}