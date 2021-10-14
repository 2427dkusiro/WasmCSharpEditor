using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    /// <summary>
    /// コードを実行する機能を提供します。
    /// </summary>
    internal class CodeExecuter
    {
        /// <summary>
        /// コードを実行します。
        /// </summary>
        /// <param name="compileResult">実行するコンパイル結果。</param>
        /// <param name="stdIn">標準入力。</param>
        /// <param name="stdOut">標準出力。</param>
        /// <param name="stdError">標準エラー出力。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static RunCodeResult RunCode(CompileResult compileResult, TextReader stdIn, TextWriter stdOut, TextWriter stdError)
        {
            Assembly asm = compileResult.Assembly ?? throw new ArgumentException("コンパイル失敗したコンパイル結果を実行することはできません", nameof(compileResult));

            MethodInfo redirectMethod = asm.GetTypes().First(type => type.Name == "__CompilerGenerated").GetMethod("__RedirectStd", BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException("コード注入が正常に行われていません。標準入出力リダイレクトメソッドが検出できません。");

            redirectMethod.Invoke(null, new object[] { stdIn, stdOut, stdError });

            MethodInfo info = compileResult.MainMethod ?? throw new ArgumentException("メインメソッドが検出できていないコンパイル結果を実行することはできません", nameof(compileResult));

            try
            {
                info.Invoke(null, null);
                return new RunCodeResult()
                {
                    IsSuccessed = true,
                    OccurredException = null,
                };
            }
            catch (TargetInvocationException ex)
            {
                return new RunCodeResult()
                {
                    IsSuccessed = false,
                    OccurredException = ex.InnerException,
                };
            }
        }
    }
}
