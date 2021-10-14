using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.IO
{
    /// <summary>
    /// ワーカーが UI と同期通信することで入力を得る <see cref="TextReader"/> を表現します。
    /// </summary>
    public class WorkerTextReader : TextReader
    {
        /// <summary>
        /// <see cref="WorkerTextReader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="onReadRequested"></param>
        /// <param name="onReadLineRequested"></param>
        public WorkerTextReader(EventHandler onReadRequested, EventHandler onReadLineRequested)
        {
            OnReadRequested += onReadRequested;
            OnReadLineRequested += onReadLineRequested;
        }

        /// <summary>
        /// 一文字分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        public event EventHandler OnReadRequested;

        /// <summary>
        /// 一行分の読み取りが要求されたときに発生するイベント。
        /// </summary>
        public event EventHandler OnReadLineRequested;

        /// <inheritdoc />
        public override int Read()
        {
            OnReadRequested?.Invoke(this, new EventArgs());
            return -1;
        }

        /// <inheritdoc />
        public override string? ReadLine()
        {
            OnReadLineRequested?.Invoke(this, new EventArgs());
            return null;
        }
    }
}
