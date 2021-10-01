using CodeRunner;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
    public class RunCodeJob
    {
        public RunCodeJob() { }

        public Guid AssemblyId { get; set; }

        public Action<string> WriteStdOutCallBack { get; set; }

        public Action<string> WriteStdErrorCallBack { get; set; }

        public RunCodeResult RunCodeResult { get; set; }

        public RunCodeStatus RunCodeStatus { get; set; }
    }

    public enum RunCodeStatus
    {
        RunWaiting,
        Running,
        Completed,
        Default,
    }
}
