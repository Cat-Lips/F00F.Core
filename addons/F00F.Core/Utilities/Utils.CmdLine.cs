using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class CmdLine
{
    public static void Parse(Network network)
    {
        ParseNetworkArgs();

        void ParseNetworkArgs()
        {
            var kids = new Queue<string>();
            var root = /*Engine.GetMainLoop() as SceneTree ?? */(GodotObject)network;

            ParseNetworkArgs();
            RunChildProcesses();

            void ParseNetworkArgs()
            {
                var savePid = Args.Has(Arg.SavePid);

                if (Args.HasInt(Arg.Id, out var id)) App.Id = id;

                if (Args.Has(Arg.Server, out var port) || Network.DedicatedServer)
                {
                    StartServer(port);
#if DEBUG
                    if (Args.HasInt(Arg.WithClients, out var count))
                        AddChildProcess(Arg.Out.Client, count);
#endif
                }
                else if (Args.Has(Arg.Client, out var host))
                {
                    CreateClient(host);
#if DEBUG
                    if (Args.Has(Arg.WithServer))
                        AddChildProcess(Arg.Out.Server);

                    if (Args.HasInt(Arg.WithClients, out var count))
                        AddChildProcess(Arg.Out.Client, count);
#endif
                }

                void StartServer(string port)
                {
                    root.CallDeferred(() =>
                    {
                        network.StartServer(port);
                        if (savePid) Run.SetReady();
                    });
                }

                void CreateClient(string host)
                {
                    root.CallDeferred(() =>
                    {
                        network.CreateClient(host);
                        if (savePid) Run.SetReady();
                    });
                }
#if DEBUG
                void AddChildProcess(string arg, int count = 1)
                {
                    for (var i = 0; i < count; ++i)
                        kids.Enqueue(arg);
                }
#endif
            }

            void RunChildProcesses()
            {
                if (kids.Count is not 0)
                    root.CallDeferred(StartNextProcess);

                void StartNextProcess()
                {
                    if (!kids.TryDequeue(out var arg)) return;
                    var pid = Run.NewInstance(arg, Arg.Out.SavePid);
                    root.CallDeferred(CheckProcessComplete);
                    App.Exit += x => OS.Kill(pid);

                    void CheckProcessComplete()
                    {
                        if (Run.IsReady(pid))
                        {
                            Run.ClearReady(pid);
                            if (kids.Count is 0)
                                root.CallDeferred(OnTasksComplete);
                            else
                                root.CallDeferred(StartNextProcess);
                            return;
                        }

                        root.CallDeferred(CheckProcessComplete);
                    }
                }

                void OnTasksComplete()
                    => DisplayServer.WindowMoveToForeground();
            }
        }
    }

    private static class Arg
    {
        public const string Id = "id";
        public const string Server = "server";
        public const string Client = "client";
        public const string SavePid = "ready";
#if DEBUG
        public const string WithServer = "with-server";
        public const string WithClients = "with-clients";
#endif
        public static class Out
        {
            public const string Server = "--server";
            public const string Client = "--client";
            public const string SavePid = "--ready";

            public static IEnumerable<string> Id(int id) => [$"--id={id}"];
            public static IEnumerable<string> Log(string file) => ["--log-file", file];
            public static IEnumerable<string> Position(in Vector2 pos) => ["--position", $"{pos.X},{pos.Y}"];
            public static IEnumerable<string> Resolution(in Vector2 size) => ["--resolution", $"{size.X}x{size.Y}"];
        }
    }

    private static class Run
    {
        private static int idx = -1;
        private static readonly Vector2I Pos1;
        private static readonly Vector2I Pos2;
        private static readonly Vector2I Pos3;
        private static readonly Vector2I Pos4;
        private static readonly Vector2I Size;

        static Run()
        {
            var posOffset = DisplayServer.WindowGetPositionWithDecorations() - DisplayServer.WindowGetPosition();
            var sizeOffset = DisplayServer.WindowGetSizeWithDecorations() - DisplayServer.WindowGetSize();
            var screenRect = DisplayServer.ScreenGetUsableRect();
            var screenCenter = screenRect.GetCenter();

            Pos1 = screenRect.Position - posOffset;
            Pos2 = new Vector2I(screenCenter.X, 0) - posOffset;
            Pos3 = new Vector2I(0, screenCenter.Y) - posOffset;
            Pos4 = screenCenter - posOffset;

            Size = screenRect.Size / 2 - sizeOffset;
        }

        public static int NewInstance(params string[] args)
        {
            var id = ++idx + 1;
            var _args = args.Concat(WindowArgs()).Concat(DebugArgs()).Concat(IdArgs()).ToArray();
            Log.Debug($"Launching new instance [args: {string.Join("|", _args)}]");
            return OS.CreateInstance(_args);

            static IEnumerable<string> WindowArgs()
            {
                var window = idx % 4 + 1;
                return window switch
                {
                    1 => WindowArgs(Pos1),
                    2 => WindowArgs(Pos2),
                    3 => WindowArgs(Pos3),
                    4 => WindowArgs(Pos4),
                    _ => throw new InvalidOperationException(ArgError()),
                };

                string ArgError()
                    => $"Expected 1-4, got {window} (for idx {idx})";

                IEnumerable<string> WindowArgs(in Vector2 pos)
                    => Arg.Out.Position(pos).Concat(Arg.Out.Resolution(Size));
            }

            IEnumerable<string> DebugArgs()
                => Arg.Out.Log($"godot.instance.{id}.log");

            IEnumerable<string> IdArgs()
                => Arg.Out.Id(id);
        }

        public static bool IsReady(int pid)
            => FS.FileExists(Ready(pid));

        public static void SetReady()
            => FS.CreateFile(Ready(OS.GetProcessId()));

        public static void ClearReady(int pid)
            => FS.RemoveFile(Ready(pid));

        private static string Ready(int pid)
            => $"user://{pid}.ready";
    }

    public static class Args
    {
        private static Dictionary<string, string> MyArgs { get; } = [];

        static Args()
        {
            GetArgs();
            LogArgs();

            static void GetArgs()
            {
                foreach (var parts in OS.GetCmdlineArgs().Select(kvp => kvp.Split('=')))
                {
                    MyArgs.Add(
                        key: parts.First().TrimStart('-').ToLower(),
                        value: parts.Skip(1).SingleOrDefault() ?? "");
                }
            }

            [Conditional("DEBUG")]
            static void LogArgs()
            {
                if (MyArgs.Count is 0)
                    return;

                Log.Debug("CmdLine:");
                foreach (var kvp in MyArgs)
                {
                    if (kvp.Value.IsNullOrEmpty())
                        Log.Debug($"  --{kvp.Key}");
                    else Log.Debug($"  --{kvp.Key}={kvp.Value}");
                }
            }
        }

        public static bool Has(string key)
            => MyArgs.ContainsKey(key.ToLower());

        public static bool Has(string key, out string value)
            => MyArgs.TryGetValue(key.ToLower(), out value);

        public static bool HasInt(string key, out int value)
        {
            if (Has(key, out var _value) && _value.IsInt(out value))
                return true;

            value = default;
            return false;
        }

        public static bool HasFloat(string key, out float value)
        {
            if (Has(key, out var _value) && _value.IsFloat(out value))
                return true;

            value = default;
            return false;
        }

        public static bool HasVec2(string key, out Vector2 value, char sep = ',')
        {
            if (Has(key, out var _value) && _value.IsVec2(out value, sep))
                return true;

            value = default;
            return false;
        }

        public static bool HasVec2I(string key, out Vector2I value, char sep = ',')
        {
            if (Has(key, out var _value) && _value.IsVec2I(out value, sep))
                return true;

            value = default;
            return false;
        }

        public static bool HasVec3(string key, out Vector3 value, char sep = ',')
        {
            if (Has(key, out var _value) && _value.IsVec3(out value, sep))
                return true;

            value = default;
            return false;
        }

        public static bool HasVec3I(string key, out Vector3I value, char sep = ',')
        {
            if (Has(key, out var _value) && _value.IsVec3I(out value, sep))
                return true;

            value = default;
            return false;
        }
    }
}
