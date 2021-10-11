using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexedDbHandler
{
    /// <summary>
    /// 非同期的にデータベースへアクセスする機能を提供することを約束します。
    /// CRUDと基本的なオプション機能の実装を約束されます。
    /// </summary>
    public interface IAsyncDbAccesser
    {
        /// <summary>
        /// 新しい要素をデータベースに追加します。
        /// </summary>
        /// <param name="key">一意のキー。</param>
        /// <param name="type">追加する要素の型を説明する文字列。</param>
        /// <param name="value">追加する要素のデータ。</param>
        /// <returns></returns>
        Task Create(string key, string type, byte[] value);

        /// <summary>
        /// データベースを開き、データベースへの接続を初期化します。
        /// </summary>
        /// <returns></returns>
        Task Open();

        /// <summary>
        /// 要素を追加または更新します。
        /// </summary>
        /// <param name="key">一意のキー。</param>
        /// <param name="type">追加または更新する要素の型を説明する文字列。</param>
        /// <param name="value">追加または更新する要素のデータ。</param>
        /// <returns></returns>
        Task Put(string key, string type, byte[] value);

        /// <summary>
        /// 要素のデータを読み取ります。
        /// </summary>
        /// <param name="key">読み取る要素の値。</param>
        /// <returns></returns>
        Task<byte[]> Read(string key);

        /// <summary>
        /// 要素を更新します。
        /// </summary>
        /// <param name="key">一意のキー。</param>
        /// <param name="type">更新する要素の型を説明する文字列。</param>
        /// <param name="value">更新する要素のデータ。</param>
        /// <returns></returns>
        Task Update(string key, string type, byte[] value);
    }
}
