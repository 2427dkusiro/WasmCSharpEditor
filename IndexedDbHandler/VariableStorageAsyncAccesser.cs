using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDbHandler
{
    /// <summary>
    /// 非同期的に変数を保存する機能へのアクセスを提供します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableStorageAsyncAccesser<T>
    {
        private readonly IAsyncDbAccesser dbAccesser;
        private readonly string name;
        private VariableStorageAsyncAccesser(IAsyncDbAccesser dbAccesser, string name)
        {
            this.dbAccesser = dbAccesser;
            this.name = name;
        }

        /// <summary>
        /// 変数の読み書きを準備して、<see cref="VariableStorageAsyncAccesser{T}"/> を取得します。
        /// </summary>
        /// <param name="dbAccesser">読み書き先のデータベースへの <see cref="IAsyncDbAccesser"/>。</param>
        /// <param name="name">読み書きする変数の名前。</param>
        /// <returns></returns>
        public static async Task<VariableStorageAsyncAccesser<T>> OpenAsync(IAsyncDbAccesser dbAccesser, string name)
        {
            var accesser = new VariableStorageAsyncAccesser<T>(dbAccesser, name);

            // 確認処理するならここ

            await dbAccesser.Open();
            return accesser;
        }

        /// <summary>
        /// 変数を非同期的に読み取ります。
        /// </summary>
        /// <returns></returns>
        public async Task<T> ReadAsync()
        {
            if (typeof(T) == typeof(string))
            {
                byte[] data = await dbAccesser.Read(name);
                string str = GetStringFromBytes(data);
                return Unsafe.As<string, T>(ref str);
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 変数を非同期的に書き込みます。
        /// </summary>
        /// <param name="value">書き込む値</param>
        /// <returns></returns>
        public async Task WriteAsync(T value)
        {
            if (typeof(T) == typeof(string))
            {
                await dbAccesser.Put(name, "str", StringToBytes(Unsafe.As<T, string>(ref value)));
                return;
            }
            throw new NotSupportedException();
        }

        private static unsafe string GetStringFromBytes(byte[] bin)
        {
            fixed (void* ptr = bin)
            {
                return new string(new ReadOnlySpan<char>(ptr, bin.Length >> 1));
            }
        }

        private static unsafe byte[] StringToBytes(string str)
        {
            int len = str.Length << 1;
            byte[] bin = new byte[len];
            fixed (void* arrPtr = bin)
            {
                fixed (void* strPtr = str)
                {
                    Buffer.MemoryCopy(strPtr, arrPtr, len, len);
                }
            }
            return bin;
        }
    }
}
