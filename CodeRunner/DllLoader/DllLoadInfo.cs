using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.DllLoader
{
    /// <summary>
    /// .NETランタイムを含む、このアプリケーションを構成するDLLファイルの情報を提供します。
    /// </summary>
    public static class DllInfoProvider
    {
        private const string indexPath = "res/DLLIndex.json";

        /// <summary>
        /// 必要なDLLファイルのファイル名を取得します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        /// <param name="cultureInfo">取得するDLLのカルチャ。</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException" />
        public static async Task<IEnumerable<string>> GetDllNames(HttpClient httpClient, CultureInfo cultureInfo)
        {
            DllLoadInfoSet? info = await httpClient.GetFromJsonAsync<DllLoadInfoSet>(indexPath);
            if (info is null)
            {
                throw new InvalidOperationException("DLLインデックスファイルの取得に失敗しました。");
            }
            return info.DllLoadInfos.Where(x => x.CultureString is null || CompareCultureInfo(x.CultureString, cultureInfo.Name)).Select(x => x.Name);
        }

        /// <summary>
        /// 必要なDLLファイルのパスを取得します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        /// <param name="cultureInfo">取得するDLLのカルチャ。</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException" />
        public static async Task<IEnumerable<string>> GetDllPaths(HttpClient httpClient, CultureInfo cultureInfo)
        {
            DllLoadInfoSet? info = await httpClient.GetFromJsonAsync<DllLoadInfoSet>(indexPath);
            if (info is null)
            {
                throw new InvalidOperationException("DLLインデックスファイルの取得に失敗しました。");
            }
            string str = DllLoadInfoSet.DirRelativePath;
            return info.DllLoadInfos.Where(x => x.CultureString is null || CompareCultureInfo(x.CultureString, cultureInfo.Name)).Select(x => $"{str}{x.Name}");
        }

        private static bool CompareCultureInfo(string? c1, string? c2)
        {
            if (c1 is null)
            {
                return c2 is null;
            }
            if (c2 is null)
            {
                return false;
            }

            string[] arr1 = c1.Split('-');
            string[] arr2 = c2.Split('-');
            return arr1.Select(x => arr2.Any(y => x == y)).Any(x => x);
        }
    }

    /// <summary>
    /// 一つのディレクトリに含まれるDLLの読み込み情報を表現します。
    /// </summary>
    public class DllLoadInfoSet
    {
        /// <summary>
        /// wwwrootからDLLがあるディレクトリへのパス。
        /// </summary>
        public static readonly string DirRelativePath = "_framework/";

        /// <summary>
        /// <see cref="DllLoadInfoSet"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="dllLoadInfos"></param>
        public DllLoadInfoSet(DllLoadInfo[] dllLoadInfos)
        {
            DllLoadInfos = dllLoadInfos ?? throw new ArgumentNullException(nameof(dllLoadInfos));
        }

        /// <summary>
        /// このセットに含まれる <see cref="DllLoadInfo"/> を取得します。
        /// </summary>
        public DllLoadInfo[] DllLoadInfos { get; }
    }

    /// <summary>
    /// DLLの読み込み情報を表現します。
    /// </summary>
    public class DllLoadInfo
    {
        /// <summary>
        /// <see cref="DllLoadInfo"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cultureString"></param>
        public DllLoadInfo(string name, string? cultureString)
        {
            Name = name;
            CultureString = cultureString;
        }

        /// <summary>
        /// このDLLのファイル名を取得または設定します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// このDLLが依存するカルチャの名前を取得または設定します。
        /// </summary>
        public string? CultureString { get; set; }
    }
}
