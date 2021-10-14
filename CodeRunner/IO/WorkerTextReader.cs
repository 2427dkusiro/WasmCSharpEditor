using JSWrapper.WorkerSyncConnection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.IO
{
    /// <summary>
    /// ワーカーが UI と同期通信することで入力を得る <see cref="TextReader"/> を表現します。
    /// </summary>
    public class WorkerTextReader : TextReader
    {
        private readonly WorkerConsoleReader workerConsoleReader;

        /// <summary>
        /// <see cref="WorkerTextReader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="onReadRequested"></param>
        /// <param name="onReadLineRequested"></param>
        /// <param name="workerConsoleReader"></param>
        public WorkerTextReader(EventHandler<Guid> onReadRequested, EventHandler<Guid> onReadLineRequested, WorkerConsoleReader workerConsoleReader)
        {
            OnReadRequested += onReadRequested;
            OnReadLineRequested += onReadLineRequested;
            this.workerConsoleReader = workerConsoleReader;
        }

        /// <summary>
        /// 一文字分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        public event EventHandler<Guid> OnReadRequested;

        /// <summary>
        /// 一行分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        public event EventHandler<Guid> OnReadLineRequested;

        /// <inheritdoc />
        public override int Read()
        {
            Guid guid = Guid.NewGuid();
            OnReadRequested?.Invoke(this, guid);
            var input = workerConsoleReader.ReadInput(guid.ToString());
            return ToInt32(input);
        }

        /// <inheritdoc />
        public override string? ReadLine()
        {
            Guid guid = Guid.NewGuid();
            OnReadLineRequested?.Invoke(this, guid);
            var input = workerConsoleReader.ReadInput(guid.ToString());
            return input;
        }

        private static int ToInt32(string str)
        {
                switch (str.Length)
                {
                    case 0:
                        return -1;
                    case 1:
                        return BitConverter.ToInt16(MemoryMarshal.AsBytes<char>(str));
                    case 2:
                        return BitConverter.ToInt32(MemoryMarshal.AsBytes<char>(str));
                    default:
                        throw new NotSupportedException("一文字の長さとして無効です。");
                }
        }
    }
}
