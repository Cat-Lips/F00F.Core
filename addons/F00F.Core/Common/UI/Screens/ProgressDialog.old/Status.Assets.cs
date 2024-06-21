using Godot;

namespace F00F;

public static class StatusConfig
{
    public static string IconPath { get; set; }
        = Utils.GetScriptDir<ProgressDialog>().PathJoin("Assets/Status{0}.svg");

    public static Color WarnColor { get; set; } = Colors.Yellow;
    public static Color ErrorColor { get; set; } = Colors.Red;
    public static Color SuccessColor { get; set; } = Colors.Green;

    public static Color? Color(this Status status) => status switch
    {
        Status.Warn => WarnColor,
        Status.Error => ErrorColor,
        Status.Success => SuccessColor,
        _ => null,
    };

    public static Texture2D Icon(this Status status) => status switch
    {
        Status.Info => null,
        _ => Utils.Load<Texture2D>(string.Format(IconPath, status)),
    };
}
