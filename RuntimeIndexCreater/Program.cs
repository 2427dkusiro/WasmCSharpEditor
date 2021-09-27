using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RuntimeIndexCreater
{
    class Program
    {
        const string output = @"C:\Users\Kota\Desktop\DLLIndex.json";
        private static readonly string[] execludes = new[]
        {
            "CodeRunner.dll",
            "WasmCsTest.dll",
        };

        static void Main(string[] args)
        {
            string path = null;
            while (true)
            {
                Console.WriteLine("Input path to '_framework' dir");
                path = Console.ReadLine();
                if (Directory.Exists(path))
                {
                    break;
                }
                Console.WriteLine("There is no such a dir");
            }

            List<CodeRunner.DllLoader.DllLoadInfo> dllLoadInfos = new List<CodeRunner.DllLoader.DllLoadInfo>();
            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (execludes.Contains(Path.GetFileName(file)))
                {
                    continue;
                }
                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                {
                    var hash = SHA256.Create().ComputeHash(fileStream);
                    var str = Convert.ToBase64String(hash);
                    dllLoadInfos.Add(new CodeRunner.DllLoader.DllLoadInfo()
                    {
                        Name = Path.GetFileName(file),
                    });
                }
            }

            CodeRunner.DllLoader.LoadInfo loadInfo = new CodeRunner.DllLoader.LoadInfo()
            {
                DllLoadInfos = dllLoadInfos.ToArray(),
            };
            var json = JsonSerializer.Serialize(loadInfo);

            using (StreamWriter streamWriter = new StreamWriter(output, false, Encoding.UTF8))
            {
                streamWriter.Write(json);
            }
            Console.WriteLine("Result was written to " + output);
        }
    }
}
