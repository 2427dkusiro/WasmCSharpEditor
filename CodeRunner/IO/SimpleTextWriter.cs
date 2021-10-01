using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeRunner.IO
{
    /// <summary>
    /// <see cref="TextWriter"/> の実装を簡略化します。<see cref="WriteString(string)"/> を実装するだけで利用できます。
    /// </summary>
    public abstract class SimpleTextWriter : TextWriter
    {
        /// <inheritdoc />
        public override Encoding Encoding => Encoding.GetEncoding("UTF-16");

        /// <inheritdoc />
        public override string NewLine
        {
            get => Environment.NewLine;
            set => _ = value;
        }

        /// <summary>
        /// 派生クラスでオーバーライドされた場合は、文字列を書き込みます。
        /// </summary>
        /// <param name="value"></param>
        public abstract void WriteString(string value);

        protected void WriteGeneric<T>(T value)
        {
            string str = value?.ToString();
            WriteString(str);
        }

        protected void WriteLineGeneric<T>(T value)
        {
            string str = value?.ToString();
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