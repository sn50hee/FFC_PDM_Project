using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace FFC_PDM
{
    internal class GetPythonModel
    {
        public void GetModel()
        {
            string contentsPath = "./contents/";
            var pythonPath = GetPythonExecutablePath();

            var processInfo = new ProcessStartInfo
            {
                FileName = pythonPath, // Python 실행 파일 경로
                Arguments = $"{contentsPath}getModelTest.py", // Python 스크립트와 파일 경로 전달
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();

                process.StandardInput.WriteLine("[[158.5692601, 390.7888887, 91.69100281, 42.63577924, 14, 0, 2, 18]]");
                process.StandardInput.Flush();
                process.StandardInput.Close();

                // 실행 결과 확인
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                Console.WriteLine("Output: " + output);
                Console.WriteLine("Error: " + error);
            }

        }

        static string GetPythonExecutablePath()
        {
            // 시스템 PATH에서 python.exe를 찾아 반환
            return GetExecutablePath("python.exe");
        }

        static string GetExecutablePath(string executableName)
        {
            var paths = Environment.GetEnvironmentVariable("PATH");
            var pathEntries = paths.Split(Path.PathSeparator);

            foreach (var pathEntry in pathEntries)
            {
                var fullPath = Path.Combine(pathEntry, executableName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
    }
}
