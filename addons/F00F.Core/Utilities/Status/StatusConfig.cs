using Godot;

namespace F00F;

public static class StatusConfig
{
    public static class Default
    {
        private static readonly string ResPath = Utils.GetResPath("Assets/Status{0}.svg");

        public static readonly Texture2D WarnIcon = Utils.Load<Texture2D>(string.Format(ResPath, Status.Warn));
        public static readonly Texture2D ErrorIcon = Utils.Load<Texture2D>(string.Format(ResPath, Status.Error));
        public static readonly Texture2D SuccessIcon = Utils.Load<Texture2D>(string.Format(ResPath, Status.Success));

        public static readonly Color WarnColor = WarnIcon.GetImage().MostCommonColor();
        public static readonly Color ErrorColor = ErrorIcon.GetImage().MostCommonColor();
        public static readonly Color SuccessColor = SuccessIcon.GetImage().MostCommonColor();
    }

    public static Texture2D WarnIcon { get; set; } = Default.WarnIcon;
    public static Texture2D ErrorIcon { get; set; } = Default.ErrorIcon;
    public static Texture2D SuccessIcon { get; set; } = Default.SuccessIcon;

    public static Color WarnColor { get; set; } = Default.WarnColor;
    public static Color ErrorColor { get; set; } = Default.ErrorColor;
    public static Color SuccessColor { get; set; } = Default.SuccessColor;

    public static Texture2D Texture(this Status status) => status switch
    {
        Status.Warn => WarnIcon,
        Status.Error => ErrorIcon,
        Status.Success => SuccessIcon,
        _ => null,
    };

    public static Color? Color(this Status status) => status switch
    {
        Status.Warn => WarnColor,
        Status.Error => ErrorColor,
        Status.Success => SuccessColor,
        _ => null,
    };

    public static Control Icon(this Status status) => UI.TextureRect("Icon", status.Texture()).SetSquareLayout();
    public static Control Text(this Status status, string msg) => UI.SunkLabel("Text", msg, status.Color());
}
