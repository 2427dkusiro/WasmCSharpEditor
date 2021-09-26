using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    public class AssemblyExecuter
    {
        public static T Execute<T>(CompileResult compileResult)
        {
            if (!compileResult.IsSuccessed)
            {
                throw new InvalidOperationException("コンパイル失敗した結果を実行することはできません");
            }

            var asm = compileResult.Assembly;
            return (T)asm.GetType("Fuga").GetMethod("Piyo").Invoke(null, null);
        }
    }
}
