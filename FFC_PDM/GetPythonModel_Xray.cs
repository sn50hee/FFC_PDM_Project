using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFC_PDM
{
    class GetPythonModel_Xray
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
                string contentsPath = Path.Combine(Directory.GetCurrentDirectory(), "contents", "kwan");
                Debug.WriteLine(contentsPath);
                var condaPath = GetPythonExecutablePath();
                // GazeTracking: 가상환경 이름
                string pythonPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(condaPath)), "envs", "kwan", "python.exe");
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
                        sw.WriteLine("conda activate kwan");
                        sw.WriteLine($"python {Path.Combine(contentsPath, "networking.py")}");
                        Debug.WriteLine($"python {Path.Combine(contentsPath, "networking.py")}");
                    }
                }
            }


        }

        public void GazeRun()
        {
            lock (this)
            {
                Debug.WriteLine("GazeRun 시작");
                string contentsPath = Path.Combine(Directory.GetCurrentDirectory(), "contents", "kwan");
                var condaPath = GetPythonExecutablePath();
                // GazeTracking: 가상환경 이름
                string pythonPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(condaPath)), "envs", "kwan", "python.exe");
                Debug.WriteLine(pythonPath);
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
                        sw.WriteLine("conda activate kwan");
                        sw.WriteLine($"python {Path.Combine(contentsPath, "detect.py")}  --weights {Path.Combine(contentsPath, "last.pt")} --source 0 --cfg {Path.Combine(contentsPath, "yolov3-spp.cfg")} --names {Path.Combine(contentsPath, "classes.names")} --output {Path.Combine(contentsPath, "result")} --device cpu");
                        Debug.WriteLine($"python {Path.Combine(contentsPath, "detect.py")}  --weights {Path.Combine(contentsPath, "last.pt")} --source 0 --cfg {Path.Combine(contentsPath, "yolov3-spp.cfg")} --names {Path.Combine(contentsPath, "classes.names")} --output {Path.Combine(contentsPath, "result")} --device cpu");

                        // --weights weights/last.pt --source 0 --cfg yolov3-spp.cfg --names classes.names --output result --device cpu
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
                    apiCheckResult = data["direction"] != null ? data["direction"].ToString() : "DefaultValue";
                    Debug.WriteLine(apiCheckResult);
                }

            }
        }

        private JToken callWebClient()
        {
            var client = new RestClient("http://127.0.0.1:5000/");
            var request = new RestRequest("get_xray_data");
            var response = client.ExecuteGet(request);
            JToken data = JToken.Parse(response.Content);
            return data;

        }

        public string GetApiCheckResult()
        {
            lock (this)
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

