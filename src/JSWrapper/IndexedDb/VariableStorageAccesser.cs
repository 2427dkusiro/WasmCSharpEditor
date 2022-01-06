using System.Runtime.CompilerServices;

namespace JSWrapper.IndexedDb
{
    /// <summary>
    /// 非同期的に変数を保存する機能へのアクセスを提供します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableStorageAccesser<T>
    {
        private readonly IDbAccesser dbAccesser;
        private readonly string name;
        private VariableStorageAccesser(IDbAccesser dbAccesser, string name)
        {
            this.dbAccesser = dbAccesser;
            this.name = name;
        }

        /// <summary>
        /// 変数の読み書きを準備して、<see cref="VariableStorageAccesser{T}"/> を取得します。
        /// </summary>
        /// <param name="dbAccesser">読み書き先のデータベースへの <see cref="IDbAccesser"/>。</param>
        /// <param name="name">読み書きする変数の名前。</param>
        /// <returns></returns>
        public static async Task<VariableStorageAccesser<T>> OpenAsync(IDbAccesser dbAccesser, string name)
        {
            var accesser = new VariableStorageAccesser<T>(dbAccesser, name);

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
                var data = await dbAccesser.Read(name);
                var str = TypeConverter.GetStringFromBytes(data);
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
                await dbAccesser.Put(name, "str", TypeConverter.StringToBytes(Unsafe.As<T, string>(ref value)));
                return;
            }
            throw new NotSupportedException();
        }
    }
}
