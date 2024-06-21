using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class StatusExtensions
{
    public static Texture2D Load(this Status status, string path)
        => Utils.Load<Texture2D>(string.Format(path, status));

    public static Color? Color(this Status status, StatusConfig cfg = null) => status switch
    {
        Status.Info => (cfg ?? StatusConfig.Default.Instance).InfoColor,
        Status.Warn => (cfg ?? StatusConfig.Default.Instance).WarnColor,
        Status.Error => (cfg ?? StatusConfig.Default.Instance).ErrorColor,
        Status.Success => (cfg ?? StatusConfig.Default.Instance).SuccessColor,
        _ => null,
    };

    public static Texture2D Texture(this Status status, StatusConfig cfg = null) => status switch
    {
        Status.Info => (cfg ?? StatusConfig.Default.Instance).InfoIcon,
        Status.Warn => (cfg ?? StatusConfig.Default.Instance).WarnIcon,
        Status.Error => (cfg ?? StatusConfig.Default.Instance).ErrorIcon,
        Status.Success => (cfg ?? StatusConfig.Default.Instance).SuccessIcon,
        _ => null,
    };

    public static Control Icon(this Status status, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Icon(status);

    public static Control Text(this Status status, string msg, StatusConfig cfg = null)
        => (cfg ?? StatusConfig.Default.Instance).Text(status, msg);

    public static void Push(this Status status, string msg, string tag = null, Action Resolve = null, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
        => IStatus.Instance.Push(status, msg, tag, Resolve, filePath, memberName);
}
