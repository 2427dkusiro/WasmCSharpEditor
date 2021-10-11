using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RuntimeIndexCreater
{
    internal class Program
    {
        private const string output = @"C:\Users\Kota\Desktop\DLLIndex.json";
        private static readonly string[] execludes = new[]
        {
            "CodeRunner.dll",
            "WasmCsTest.dll",
        };

        private static void Main(string[] args)
        {
            string frameworkDirPath = null;
            if (args.Length == 1)
            {
                string arg = args[0];
                if (Directory.Exists(arg))
                {
                    frameworkDirPath = arg;
                }
                else
                {
                    Console.WriteLine($"与えられた引数\"{arg}\"は有効なパスではありません");
                }
            }

            if (frameworkDirPath is null)
            {
                while (true)
                {
                    Console.WriteLine("Input path to '_framework' dir");
                    frameworkDirPath = Console.ReadLine();
                    if (frameworkDirPath.StartsWith("\""))
                    {
                        frameworkDirPath = frameworkDirPath[1..^1];
                    }
                    if (Directory.Exists(frameworkDirPath))
                    {
                        break;
                    }
                    Console.WriteLine("There is no such a dir");
                }
            }

            var dllLoadInfos = new List<CodeRunner.DllLoader.DllLoadInfo>();
            string[] files = Directory.GetFiles(frameworkDirPath, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (execludes.Contains(Path.GetFileName(file)))
                {
                    continue;
                }
                using (var fileStream = new FileStream(file, FileMode.Open))
                {
                    byte[] hash = SHA256.Create().ComputeHash(fileStream);
                    string str = Convert.ToBase64String(hash);
                    dllLoadInfos.Add(new CodeRunner.DllLoader.DllLoadInfo()
                    {
                        Name = Path.GetFileName(file),
                        CultureString = null,
                    });
                }
            }

            /* カルチャ依存DLL読み込み部(保留)
            string[] dirs = Directory.GetDirectories(frameworkDirPath);
            foreach (string dir in dirs)
            {
                string name = Path.GetFileName(dir);
                CultureInfo cultureInfo;
                try
                {
                    cultureInfo = CultureInfo.GetCultureInfo(name);
                }
                catch (CultureNotFoundException)
                {
                    continue;
                }
                string[] cfiles = Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly);
                foreach (string cfile in cfiles)
                {
                    if (execludes.Contains(Path.GetFileName(cfile)))
                    {
                        continue;
                    }

                    string newPath = $"{frameworkDirPath}\\{cultureInfo.Name}_{Path.GetFileName(cfile)}";
                    File.Copy(cfile, newPath);

                    dllLoadInfos.Add(new CodeRunner.DllLoader.DllLoadInfo()
                    {
                        Name = Path.GetFileName(newPath),
                        CultureString = cultureInfo.Name,
                    });
                }
            }
            */

            var loadInfo = new CodeRunner.DllLoader.LoadInfo()
            {
                DllLoadInfos = dllLoadInfos.ToArray(),
            };
            string json = JsonSerializer.Serialize(loadInfo);

            using (var streamWriter = new StreamWriter(output, false, Encoding.UTF8))
            {
                streamWriter.Write(json);
            }
            Console.WriteLine("Result was written to " + output);
        }
    }
}
