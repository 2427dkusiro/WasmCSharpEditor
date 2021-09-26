using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
    public abstract class SimpleTextWriter : TextWriter
    {
        public override Encoding Encoding => throw new NotImplementedException();

        public override string NewLine
        {
            get => Environment.NewLine;
            set => _ = value;
        }

        public abstract void WriteString(string value);

        public void WriteGeneric<T>(T value)
        {
            var str = value.ToString();
            WriteString(str);
        }

        public void WriteLineGeneric<T>(T value)
        {
            var str = value.ToString();
            WriteString(str + NewLine);
        }

        #region Writes
        /// <inheritdoc/>
        public override void Write(bool value)
        {
            WriteGeneric(value);
        }

        public override void Write(char value)
        {
            WriteGeneric(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            WriteGeneric(new string(buffer, index, count));
        }

        public override void Write(char[] buffer)
        {
            WriteGeneric(new string(buffer));
        }

        public override void Write(decimal value)
        {
            WriteGeneric(value);
        }

        public override void Write(double value)
        {
            WriteGeneric(value);
        }

        public override void Write(float value)
        {
            WriteGeneric(value);
        }

        public override void Write(int value)
        {
            WriteGeneric(value);
        }

        public override void Write(long value)
        {
            WriteGeneric(value);
        }

        public override void Write(object value)
        {
            WriteGeneric(value);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            WriteGeneric(new string(buffer));
        }

        public override void Write(string format, object arg0)
        {
            WriteGeneric(string.Format(format, arg0));
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WriteGeneric(string.Format(format, arg0, arg1));
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteGeneric(string.Format(format, arg0, arg1, arg2));
        }

        public override void Write(string format, params object[] arg)
        {
            WriteGeneric(string.Format(format, arg));
        }

        public override void Write(string value)
        {
            WriteGeneric(value);
        }

        public override void Write(StringBuilder value)
        {
            WriteGeneric(value);
        }

        public override void Write(uint value)
        {
            WriteGeneric(value);
        }

        public override void Write(ulong value)
        {
            WriteGeneric(value);
        }
        #endregion

        #region WriteAsyncs(未実装)

        public override Task WriteAsync(char value)
        {
            return base.WriteAsync(value);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return base.WriteAsync(buffer, index, count);
        }

        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return base.WriteAsync(buffer, cancellationToken);
        }

        public override Task WriteAsync(string value)
        {
            return base.WriteAsync(value);
        }

        public override Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)
        {
            return base.WriteAsync(value, cancellationToken);
        }

        #endregion

        #region WriteLines

        public override void WriteLine()
        {
            WriteString(NewLine);
        }

        public override void WriteLine(bool value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(char value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteLineGeneric(new string(buffer, index, count));
        }

        public override void WriteLine(char[] buffer)
        {
            WriteLineGeneric(new string(buffer));
        }

        public override void WriteLine(decimal value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(double value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(float value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(int value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(long value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(object value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            WriteLineGeneric(new string(buffer));
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteLineGeneric(string.Format(format, arg0));
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteLineGeneric(string.Format(format, arg0, arg1));
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLineGeneric(string.Format(format, arg0, arg1, arg2));
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteLineGeneric(string.Format(format, arg));
        }

        public override void WriteLine(string value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(StringBuilder value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(uint value)
        {
            WriteLineGeneric(value);
        }

        public override void WriteLine(ulong value)
        {
            WriteLineGeneric(value);
        }

        #endregion
    }
}