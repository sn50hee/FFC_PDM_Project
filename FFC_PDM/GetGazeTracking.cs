using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;

namespace FFC_PDM
{
    class GetGazeTracking
    {
        private string apiCheckResult = "";
        public void GazeTracking()
        {
            Thread openServer = new Thread(OpenServer);
            Thread gazeRun = new Thread(GazeRun);
            Thread apiCheck = new Thread(ApiCheck);

            openServer.IsBackground = true;
            gazeRun.IsBackground = true;
            apiCheck.IsBackground = true;

            openServer.Start();
            gazeRun.Start();

            Thread apiCheckThread = new Thread(() =>
            {
                while (true)
                {
                    ApiCheck();
                    Thread.Sleep(1000); // 1초 대기
                }
            });
            apiCheckThread.IsBackground = true;
            apiCheckThread.Start();
        }

        public void OpenServer()
        {
            
            lock (this)
            {
                Debug.WriteLine("OpenServer 시작");
                string contentsPath = Path.Combine(Directory.GetCurrentDirectory(), "contents", "GazeTracking");
                var condaPath = GetPythonExecutablePath();
                // GazeTracking: 가상환경 이름
                string pythonPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(condaPath)), "envs", "GazeTracking", "python.exe");
                List<string> outputs = new List<string>();

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                using (var sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(condaPath);
                        sw.WriteLine("conda activate GazeTracking");
                        sw.WriteLine($"python {Path.Combine(contentsPath, "gaze_server.py")}");
                    }
                }
            }
            
            
        }

        public void GazeRun()
        {
            lock (this)
            {
                Debug.WriteLine("GazeRun 시작");
                string contentsPath = Path.Combine(Directory.GetCurrentDirectory(), "contents", "GazeTracking");
                var condaPath = GetPythonExecutablePath();
                // GazeTracking: 가상환경 이름
                string pythonPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(condaPath)), "envs", "GazeTracking", "python.exe");
                List<string> outputs = new List<string>();

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                using (var sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(condaPath);
                        sw.WriteLine("conda activate GazeTracking");
                        sw.WriteLine($"python {Path.Combine(contentsPath, "run.py")}");
                    }
                }
            }
            

        }

        public void ApiCheck()
        {
            lock (this)
            {
                Debug.WriteLine("ApiCheck 시작");
                JToken data = callWebClient();
                //Debug.WriteLine(data.ToString());
                if (data != null)
                {
                    apiCheckResult = data["direction"].ToString();
                    Debug.WriteLine(apiCheckResult);
                }

            }
        }

        private JToken callWebClient()
        {
            var client = new RestClient("http://127.0.0.1:5000/");
            var request = new RestRequest("get_gaze_data");
            var response = client.ExecuteGet(request);
            JToken data = JToken.Parse(response.Content);
            return data;

        }

        public string GetApiCheckResult()
        {
            //lock (this)
            {
                return apiCheckResult;
            }
        }

        static string GetPythonExecutablePath()
        {
            return GetExecutablePath("activate.bat");
        }

        static string GetExecutablePath(string executableName)
        {
            var paths = Environment.GetEnvironmentVariable("PATH");
            var pathEntries = paths.Split(Path.PathSeparator);

            foreach (var pathEntry in pathEntries)
            {
                var fullPath = Path.Combine(pathEntry, executableName);
                if (fullPath.Contains("anaconda3\\Scripts"))
                {
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                
            }

            return null;
        }
    }
}
