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

        public static async Task<IEnumerable<string>> GetDllNames(HttpClient httpClient)
        {
            LoadInfo info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            return info.DllLoadInfos.Select(x => x.Name);
        }

        public static async Task<IEnumerable<string>> GetDllPaths(HttpClient httpClient)
        {
            LoadInfo info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            string str = LoadInfo.DirRelativePath;
            return info.DllLoadInfos.Select(x => $"{str}{x.Name}");
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
