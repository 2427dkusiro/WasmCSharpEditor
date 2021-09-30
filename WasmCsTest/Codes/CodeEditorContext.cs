using CodeRunner;

using System;
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
            var job = new CompileJob()
            {
                Code = code,
            };
            compileJob = job;
            await compileQueueService.CompileAsync(job);
        }

        private CompileJob compileJob;

        /// <summary>
        /// コンパイルの状態を取得します。
        /// </summary>
        public CompileStatus CompileState { get => compileJob?.CompileState ?? CompileStatus.Default; }

        /// <summary>
        /// コンパイルの結果を取得します。
        /// </summary>
        public CompilerResultMessage CompileResult { get => compileJob?.CompileResult; }

        /// <summary>
        /// コンパイルにかかった時間を取得します。
        /// </summary>
        public TimeSpan CompileTime { get => compileJob.CompileTime; }
    }
}