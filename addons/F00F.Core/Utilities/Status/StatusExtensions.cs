using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using StatusMsg = Action<Status, string>;

public static class StatusExtensions
{
    public static Texture2D Load(this Status? status, string path) => status?.Load(path);
    public static Texture2D Load(this Status status, string path) => Utils.Load<Texture2D>(string.Format(path, status));

    public static Color? Color(this Status? status, StatusConfig cfg = null) => status?.Color(cfg);
    public static Color? Color(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).Color(status);
    public static Color? BackColor(this Status? status, StatusConfig cfg = null) => status?.BackColor(cfg);
    public static Color? BackColor(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).BackColor(status);
    public static Color Modulate(this Status? status, StatusConfig cfg = null) => status?.Modulate(cfg) ?? Const.ModulateDflt;
    public static Color Modulate(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).Modulate(status);

    public static Color? TextColor(this Status? status, StatusConfig cfg = null) => status?.TextColor(cfg);
    public static Color? TextColor(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).TextColor(status);
    public static Color? FontColor(this Status? status, StatusConfig cfg = null) => status?.FontColor(cfg);
    public static Color? FontColor(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).FontColor(status);
    public static Color? ForeColor(this Status? status, StatusConfig cfg = null) => status?.ForeColor(cfg);
    public static Color? ForeColor(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).ForeColor(status);

    public static Texture2D Texture(this Status? status, StatusConfig cfg = null) => status?.Texture(cfg);
    public static Texture2D Texture(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).Texture(status);

    public static Control StatusIcon(this Status? status, StatusConfig cfg = null) => status?.StatusIcon(cfg);
    public static Control StatusIcon(this Status status, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).StatusIcon(status);

    public static Control StatusLabel(this Status? status, string msg, StatusConfig cfg = null) => status?.StatusLabel(msg, cfg);
    public static Control StatusLabel(this Status status, string msg, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).StatusLabel(status, msg);

    public static Control StatusButton(this Status? status, string msg, Action action, StatusConfig cfg = null) => status?.StatusButton(msg, action, cfg);
    public static Control StatusButton(this Status status, string msg, Action action, StatusConfig cfg = null) => (cfg ?? StatusConfig.Default.Instance).StatusButton(status, msg, action);

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
