using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    internal class CodeExecuter
    {
        public static async Task RunCode(CompileResult compileResult, TextReader stdIn, TextWriter stdOut, TextWriter stdError)
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
            info.Invoke(null, null);
        }
    }
}
