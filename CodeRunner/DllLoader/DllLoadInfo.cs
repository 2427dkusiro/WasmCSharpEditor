using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.DllLoader
{
    public class LoadInfo
    {
        public static readonly string DirRelativePath = "_framework/";

        public DllLoadInfo[] DllLoadInfos { get; set; }
    }

    public class DllLoadInfo
    {
        public string Name { get; set; }

        public string Hash { get; set; }
    }
}
