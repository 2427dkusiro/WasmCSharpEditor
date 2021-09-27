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
@"using System;
class Program
{
    static void Main()
    {
        Console.WriteLine(DateTime.Now);
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
