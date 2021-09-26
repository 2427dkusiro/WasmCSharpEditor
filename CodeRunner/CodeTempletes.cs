using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    public static class CodeTempletes
    {
        public static readonly string GetNowTimeSampleCode =
@"public class Fuga
{
    public static string Piyo()
    {
        return System.DateTime.Now.ToString();
    }
}";

        public static readonly string TempleteCode =
@"using System;
class Program
{
    static void Main()
    {
        // Write your Code here
    }
}";
    }
}
