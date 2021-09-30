using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
    /// <summary>
    /// JavascriptのイベントからC#のイベントへの変換を支援する機能を提供します。
    /// </summary>
    /// <remarks>
    /// このクラスは以下のように機能します。
    /// 1. javascriptにguidを文字列として渡します。guidは、どのjavascriptのイベントを購読するかを区別するのに使います。
    /// 2. イベントハンドラを、guidとともに <see cref="AddHandler(Guid, EventHandler)"/> に渡してイベントを購読します。
    /// 3. javascriptはイベントを受け取ると、<see cref="RaiseOnChangeEventFromJs(string)"/> をguidを引数として呼び出します。
    /// 4. このクラスは、guidに関連付けられたすべてのイベントハンドラを実行します。 
    /// </remarks>
    public class EventWrapper
    {
        private static readonly Dictionary<Guid, List<EventHandler>> dictionary = new();

        /// <summary>
        /// このメソッドはjavascriptから呼び出されます。
        /// </summary>
        /// <param name="guid"></param>
        [JSInvokable]
        public static void RaiseOnChangeEventFromJs(string guid)
        {
            var key = Guid.Parse(guid);
            if (!dictionary.ContainsKey(key))
            {
                return;
            }

            foreach (EventHandler handler in dictionary[key])
            {
                handler?.Invoke(null, new EventArgs());
            }
        }

        /// <summary>
        /// イベントハンドラを登録します。
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="eventHandler"></param>
        public static void AddHandler(Guid guid, EventHandler eventHandler)
        {
            if (dictionary.ContainsKey(guid))
            {
                dictionary[guid].Add(eventHandler);
            }
            else
            {
                dictionary.Add(guid, new List<EventHandler>() { eventHandler });
            }
        }

        /// <summary>
        /// イベントハンドラを削除します。
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="eventHandler"></param>
        public static void RemoveHandler(Guid guid, EventHandler eventHandler)
        {
            if (dictionary.ContainsKey(guid))
            {
                dictionary[guid].Remove(eventHandler);
                if (dictionary[guid].Count == 0)
                {
                    dictionary.Remove(guid);
                }
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
