using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.IO
{
    /// <summary>
    /// イベントを用いて間接的にテキスト書き込みを行う <see cref="System.IO.TextWriter"/> を表現します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class EventTextWriter : SimpleTextWriter
    {
        /// <summary>
        /// <see cref="EventTextWriter"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public EventTextWriter() { }

        /// <summary>
        /// <see cref="EventTextWriter"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="eventHandler">初期値として登録するイベント。</param>
        public EventTextWriter(EventHandler<string> eventHandler)
        {
            WriteRequested += eventHandler;
        }

        /// <summary>
        /// 書き込みが要求されたときに発生するイベント。
        /// </summary>
        public event EventHandler<string> WriteRequested;

        /// <summary>
        /// イベントを発生させ、文字列の書き込みを要求します。
        /// </summary>
        /// <param name="value"></param>
        public override void WriteString(string value)
        {
            WriteRequested?.Invoke(this, value);
        }
    }
}
