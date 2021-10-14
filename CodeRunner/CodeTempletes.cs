using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    /// <summary>
    /// C#コードのテンプレートを提供します。
    /// </summary>
    public static class CodeTempletes
    {
        /// <summary>
        /// 現在時刻を表示するサンプルコードを取得します。
        /// </summary>
        public static readonly string GetNowTimeSampleCode =
@"using System;
class Program
{
    static void Main()
    {
        Console.WriteLine(DateTime.Now);
    }
}";

        /// <summary>
        /// 空のMain関数を含むサンプルコードを取得します。
        /// </summary>
        public static readonly string TempleteCode =
@"using System;
class Program
{
    static void Main()
    {
        // Write your Code here
    }
}";

        /// <summary>
        /// コンパイラとC#言語のバージョンを取得するコードを取得します。
        /// </summary>
        public static readonly string GetVersionCode =
@"using System;
class Program
{
    static void Main()
    {
        #error version
    }
}";
    }
}
