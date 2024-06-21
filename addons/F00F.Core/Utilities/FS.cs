using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

namespace F00F
{
    public static class FS
    {
        public static bool Exists(string path) => DirExists(path) || FileExists(path);
        public static bool DirExists(string path) => DirAccess.DirExistsAbsolute(path);
        public static bool FileExists(string path) => FileAccess.FileExists(path);

        public static void MakeDir(string path) => DirAccess.MakeDirRecursiveAbsolute(path).Throw(nameof(MakeDir), path);
        public static void Rename(string path, string to) => DirAccess.RenameAbsolute(path, to).Throw(nameof(Rename), path);

        public static string[] GetFiles(string path) => DirAccess.GetFilesAt(path);
        public static string[] GetFiles(string path, bool hidden)
        {
            using (var dir = DirAccess.Open(path))
            {
                dir.IncludeHidden = hidden;
                return dir.GetFiles();
            }
        }

        public static string[] GetDirs(string path) => DirAccess.GetDirectoriesAt(path);
        public static string[] GetDirs(string path, bool hidden)
        {
            using (var dir = DirAccess.Open(path))
            {
                dir.IncludeHidden = hidden;
                return dir.GetDirectories();
            }
        }

        public static void Remove(string path) => DirAccess.RemoveAbsolute(path).Throw(nameof(Remove), path);
        public static void RemoveFile(string path, bool force)
        {
            if (force) FileAccess.SetReadOnlyAttribute(path, false).Throw(nameof(Remove), path);
            Remove(path);
        }

        public static void RemoveDir(string path)
        {
            GetFiles(path, hidden: true).ForEach(file => RemoveFile(path.PathJoin(file), force: true));
            GetDirs(path, hidden: true).ForEach(dir => RemoveDir(path.PathJoin(dir)));
            Remove(path);
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            if (FileExists(path))
            {
                using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
                {
                    FileAccess.GetOpenError().Throw("File Read", path);
                    while (file.GetPosition() < file.GetLength())
                        yield return file.GetLine();
                }
            }
        }

        public static void WriteLines(string path, IEnumerable<string> lines)
        {
            using (var file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead))
            {
                FileAccess.GetOpenError().Throw("File Write", path);
                lines.ForEach(file.StoreLine);
            }
        }

        public static byte[] ReadBytes(string path)
        {
            var bytes = FileAccess.GetFileAsBytes(path);
            FileAccess.GetOpenError().Throw("File Read", path);
            return bytes;
        }

        public static void WriteBytes(string path, byte[] bytes)
        {
            using (var file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead))
            {
                FileAccess.GetOpenError().Throw("File Write", path);
                file.StoreBuffer(bytes);
            }
        }

        public delegate void ExeCancel();
        public delegate void ExeComplete(int exitCode);
        public delegate void ExeProgress(string output, bool error);
        public static void Execute(string cmd, ExeProgress Progress = null, ExeComplete Complete = null) => Execute(null, cmd, out var _, Progress, Complete);
        public static void Execute(string dir, string cmd, ExeProgress Progress = null, ExeComplete Complete = null) => Execute(dir, cmd, out var _, Progress, Complete);
        public static void Execute(string cmd, out ExeCancel Cancel, ExeProgress Progress = null, ExeComplete Complete = null) => Execute(null, cmd, out Cancel, Progress, Complete);
        public static void Execute(string dir, string cmd, out ExeCancel Cancel, ExeProgress Progress = null, ExeComplete Complete = null)
        {
            cmd.SplitCmd(out cmd, out var args);

            var s = StartInfo();
            var p = Process(s);
            Cancel = p.Close;

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            ProcessStartInfo StartInfo() => new(cmd, args)
            {
                CreateNoWindow = true,
                WorkingDirectory = dir is null ? null : ProjectSettings.GlobalizePath(dir),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            Process Process(ProcessStartInfo startInfo)
            {
                var p = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                p.OutputDataReceived += OnOutput;
                p.ErrorDataReceived += OnError;
                p.Exited += OnExit;
                return p;

                void OnExit(object sender, EventArgs e) => CallOnMainThread(() => Complete?.Invoke(p.ExitCode));
                void OnError(object sender, DataReceivedEventArgs e) => CallOnMainThread(() => Progress?.Invoke(e.Data, true));
                void OnOutput(object sender, DataReceivedEventArgs e) => CallOnMainThread(() => Progress?.Invoke(e.Data, false));
                static void CallOnMainThread(Action action)
                {
                    Debug.Assert(OS.GetThreadCallerId() != OS.GetMainThreadId());
                    Callable.From(action).CallDeferred();
                }
            }
        }

        public delegate void HttpCancel();
        public delegate void HttpComplete(byte[] response);
        public delegate void HttpProgress(string output, bool error);
        public static void Request(string url, HttpProgress Progress = null, HttpComplete Complete = null) => Request(url, out var _, Progress, Complete);
        public static void Request(string url, out HttpCancel Cancel, HttpProgress Progress = null, HttpComplete Complete = null)
        {
            var http = new HttpClient();
            var response = new List<byte[]>();
            var byteCount = 0;

            url.SplitUrl(out var host, out var request);
            ConnectToHostAsync();
            Cancel = http.Close;

            void ConnectToHostAsync()
            {
                ReportProgress($"Connecting to {host}");
                http.ConnectToHost(host).Throw(nameof(Request), host);
                Poll(OnConnectToHostComplete);

                void OnConnectToHostComplete(HttpClient.Status status)
                {
                    switch (IsConnected())
                    {
                        case true: MakeRequestAsync(); break;
                        case false: Poll(OnConnectToHostComplete); break;
                        case null: OnError($"{status}".Capitalize()); break;
                    }

                    bool? IsConnected() => status switch
                    {
                        HttpClient.Status.Resolving => false,
                        HttpClient.Status.Connecting => false,
                        HttpClient.Status.Connected => true,
                        _ => null,
                    };
                }
            }

            void MakeRequestAsync()
            {
                ReportProgress($"Requesting {request}");
                http.Request(HttpClient.Method.Get, request, null).Throw(nameof(Request), request);
                Poll(OnRequestComplete);

                void OnRequestComplete(HttpClient.Status status)
                {
                    switch (IsComplete())
                    {
                        case true: GetResponseAsync(); break;
                        case false: Poll(OnRequestComplete); break;
                        case null: OnError($"{status}".Capitalize()); break;
                    }

                    bool? IsComplete() => status switch
                    {
                        HttpClient.Status.Requesting => false,
                        HttpClient.Status.Body => true,
                        _ => null,
                    };
                }
            }

            void GetResponseAsync()
            {
                ReadChunks();
                Poll(OnChunksComplete);

                void ReadChunks()
                {
                    while (http.GetStatus() is HttpClient.Status.Body)
                    {
                        var chunk = http.ReadResponseBodyChunk();
                        if (chunk.Length is 0) break;
                        byteCount += chunk.Length;
                        response.Add(chunk);
                        OnProgress();
                    }
                }

                void OnChunksComplete(HttpClient.Status status)
                {
                    switch (IsComplete())
                    {
                        case true: OnComplete(); break;
                        case false: GetResponseAsync(); break;
                        case null: OnError($"{status}".Capitalize()); break;
                    }

                    bool? IsComplete() => status switch
                    {
                        HttpClient.Status.Connected => IsOk(),
                        HttpClient.Status.Body => false,
                        _ => null,
                    };

                    bool IsOk() => http.GetResponseCode() is (int)HttpClient.ResponseCode.Ok;
                }
            }

            void Poll(Action<HttpClient.Status> action)
            {
                Callable.From(Poll).CallDeferred();

                void Poll()
                {
                    http.Poll().Throw(nameof(Request), url);
                    action?.Invoke(http.GetStatus());
                }
            }

            void OnError(string msg)
            {
                ReportProgress(msg, true);
                http.Close();
            }

            void OnComplete()
            {
                Complete?.Invoke(response.SelectMany(x => x).ToArray());
                http.Close();
            }

            void OnProgress()
            {
                var bodySize = http.GetResponseBodyLength();
                ReportProgress($"Receiving {request} [{Progress()}]");

                string Progress() => bodySize > 0
                    ? $"{byteCount / bodySize:P0}"
                    : Utils.HumaniseBytes((ulong)response.Count);
            }

            void ReportProgress(string msg, bool error = false)
                => Progress?.Invoke(msg, error);
        }

        public delegate void HttpResponse(Dictionary response);
        public static void RequestJson(string url, HttpProgress Progress = null, HttpResponse Response = null) => RequestJson(url, out var _, Progress, Response);
        public static void RequestJson(string url, out HttpCancel Cancel, HttpProgress Progress = null, HttpResponse Response = null)
        {
            Request(url, out Cancel, Progress, OnComplete);

            void OnComplete(byte[] bytes)
            {
                Response?.Invoke(GetResponse());

                Dictionary GetResponse()
                {
                    var json = new Json();
                    json.Parse(bytes.GetStringFromUtf8());
                    return json.Data.AsGodotDictionary();
                }
            }
        }

        public delegate void HttpDownloadComplete(string file);
        public static void RequestFile(string url, string dir, HttpProgress Progress = null, HttpDownloadComplete Response = null) => RequestFile(url, dir, out var _, Progress, Response);
        public static void RequestFile(string url, string dir, out HttpCancel Cancel, HttpProgress Progress = null, HttpDownloadComplete Response = null)
        {
            Request(url, out Cancel, Progress, OnComplete);

            void OnComplete(byte[] bytes)
            {
                var file = dir.PathJoin(url.GetFile());
                WriteBytes(file, bytes);
                Response?.Invoke(file);
            }
        }

        public static int Extract(string zip, string dir, Func<string, string> ExtractAs)
        {
            ExtractAs ??= x => x;
            dir ??= zip.GetBaseDir();

            var count = 0;
            ExtractFiles();
            return count;

            void ExtractFiles()
            {
                using (var unzip = new ZipReader())
                {
                    unzip.Open(zip).Throw("Zip Open", zip);
                    foreach (var source in unzip.GetFiles())
                    {
                        var target = ExtractAs(source);
                        if (target is null) continue;

                        target = dir.PathJoin(target);
                        ExtractFile(source, target);
                    }

                    void ExtractFile(string source, string target)
                    {
                        if (IsDir()) return;

                        var bytes = unzip.ReadFile(source);
                        MakeDir(target.GetBaseDir());
                        WriteBytes(target, bytes);
                        ++count;

                        bool IsDir()
                            => source.EndsWith('/');
                    }
                }
            }
        }

        public static int Extract(string zip, string dir = null, string root = null)
        {
            root = root?.AddSuffix("/");
            return Extract(zip, dir, root is null ? null : ExtractAs);

            string ExtractAs(string source)
            {
                var parts = source.Split(root, count: 2);
                return parts.Length is 2 ? parts.Last() : null;
            }
        }

        public static int Extract(string zip, string dir, string root, string dump)
        {
            root = root?.AddSuffix("/");
            dump = dump?.AddSuffix("/");
            return Extract(zip, dir, root is null ? null : ExtractAs);

            string ExtractAs(string source)
            {
                var parts = source.Split(root, count: 2);
                return parts.Length is 2 ? parts.Last() : DumpFile();

                string DumpFile()
                    => dump.PathJoin(parts.Single());
            }
        }
    }
}
