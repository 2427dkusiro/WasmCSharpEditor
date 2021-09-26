using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WasmCsTest.Codes;

namespace WasmCsTest.Components
{
    public partial class VirtualConsole
    {

    }

    public class VirtualConsoleWriter : SimpleTextWriter
    {
        private readonly Action<string> action;

        public VirtualConsoleWriter(Action<string> action)
        {
            this.action = action;
        }

        public override void WriteString(string value)
        {
            action?.Invoke(value);
        }
    }
}
