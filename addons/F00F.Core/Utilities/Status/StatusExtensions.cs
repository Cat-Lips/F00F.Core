using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using StatusMsg = Action<Status, string>;

public static class StatusExtensions
{
    public static Texture2D Load(this Status status, string path)
        => Utils.Load<Texture2D>(string.Format(path, status));

    public static Color? Color(this Status status, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Color(status);

    public static Texture2D Texture(this Status status, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Texture(status);

    public static Control Icon(this Status status, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Icon(status);

    public static Control Text(this Status status, string msg, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Text(status, msg);

    public static void Push(this Status status, string msg, string tag = null, Action Resolve = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
    {
        LogMsg();
        Notify();
        FwdMsg();

        void LogMsg()
        {
            switch (status)
            {
                case Status.Error:
                    Log.Error($"[{tag}] {msg}", filePath, memberName);
                    break;
                case Status.Warn:
                    Log.Warn($"[{tag}] {msg}", filePath, memberName);
                    break;
                default:
                    Log.Debug($"[{tag}] {msg} [{status}]", filePath, memberName);
                    break;
            }
        }

        void Notify()
        {
            StatusMsg sub;

            if (tag.NotNullOrEmpty())
            {
                if (Subs.TryGetValue(tag, out sub))
                    sub?.Invoke(status, msg);
            }

            if (Subs.TryGetValue(string.Empty, out sub))
                sub?.Invoke(status, msg);
        }

        void FwdMsg()
            => IStatus.Instance?.Push(status, msg, tag, Resolve);
    }

    private static readonly Dictionary<string, StatusMsg> Subs = [];
    internal static void Subscribe(string tag, StatusMsg action, bool subscribe = true)
    {
        tag ??= string.Empty;

        if (subscribe)
        {
            if (!Subs.TryGetValue(tag, out var sub))
                Subs.Add(tag, action);
            else sub += action;
        }
        else
        {
            if (Subs.TryGetValue(tag, out var sub))
                sub -= action;
        }
    }
}
