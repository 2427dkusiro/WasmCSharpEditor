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
    public static class DllInfoProvider
    {
        private const string indexPath = "res/DLLIndex.json";

        public static async Task<IEnumerable<string>> GetDllNames(HttpClient httpClient, CultureInfo cultureInfo)
        {
            LoadInfo info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            return info.DllLoadInfos.Where(x => x.CultureString is null || CompareCultureInfo(x.CultureString, cultureInfo.Name)).Select(x => x.Name);
        }

        public static async Task<IEnumerable<string>> GetDllPaths(HttpClient httpClient, CultureInfo cultureInfo)
        {
            LoadInfo info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            string str = LoadInfo.DirRelativePath;
            return info.DllLoadInfos.Where(x => x.CultureString is null || CompareCultureInfo(x.CultureString, cultureInfo.Name)).Select(x => $"{str}{x.Name}");
        }

        private static bool CompareCultureInfo(string c1, string c2)
        {
            if (c1 is null)
            {
                return c2 is null;
            }
            if (c2 is null)
            {
                return false;
            }

            var arr1 = c1.Split('-');
            var arr2 = c2.Split('-');
            return arr1.Select(x => arr2.Any(y => x == y)).Any(x => x);
        }
    }

    public class LoadInfo
    {
        public static readonly string DirRelativePath = "_framework/";

        public DllLoadInfo[] DllLoadInfos { get; set; }
    }

    public class DllLoadInfo
    {
        public string Name { get; set; }

        public string CultureString { get; set; }
    }
}
