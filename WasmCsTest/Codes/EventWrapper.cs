using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
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
            Guid key = Guid.Parse(guid);
            if (!dictionary.ContainsKey(key))
            {
                return;
            }

            foreach (var handler in dictionary[key])
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
