using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    internal class CodeExecuter
    {
        public static async Task<RunCodeResult> RunCode(CompileResult compileResult, TextReader stdIn, TextWriter stdOut, TextWriter stdError)
        {
            if (compileResult is null)
            {
                throw new ArgumentNullException(nameof(compileResult));
            }

            Assembly asm = compileResult.Assembly;
            if (asm is null)
            {
                throw new ArgumentException("コンパイル失敗したコンパイル結果を実行することはできません", nameof(compileResult));
            }

            MethodInfo redirectMethod = asm.GetTypes().First(type => type.Name == "__CompilerGenerated").GetMethod("__RedirectStd", BindingFlags.Public | BindingFlags.Static);
            redirectMethod.Invoke(null, new object[] { stdIn, stdOut, stdError });

            MethodInfo info = compileResult.MainMethod;
            if (info is null)
            {
                throw new ArgumentException("メインメソッドが検出できていないコンパイル結果を実行することはできません", nameof(compileResult));
            }
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
