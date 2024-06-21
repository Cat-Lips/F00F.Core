using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class StatusConfig : CustomResource
{
    #region Defaults

    public static class Default
    {
        private static readonly string ResPath = Utils.GetResPath("Assets/Status{0}.svg");

        public static readonly Color InfoColor = default;
        public static readonly Color WarnColor = WarnIcon.GetImage().MostCommonColor();
        public static readonly Color ErrorColor = ErrorIcon.GetImage().MostCommonColor();
        public static readonly Color SuccessColor = SuccessIcon.GetImage().MostCommonColor();

        public static Texture2D InfoIcon => field ??= default;
        public static Texture2D WarnIcon => field ??= Status.Warn.Load(ResPath);
        public static Texture2D ErrorIcon => field ??= Status.Error.Load(ResPath);
        public static Texture2D SuccessIcon => field ??= Status.Success.Load(ResPath);

        public static StatusConfig Instance => field ??= new();
    }

    #endregion

    #region Export

    [Export] public Texture2D InfoIcon { get; set; } = Default.InfoIcon;
    [Export] public Texture2D WarnIcon { get; set; } = Default.WarnIcon;
    [Export] public Texture2D ErrorIcon { get; set; } = Default.ErrorIcon;
    [Export] public Texture2D SuccessIcon { get; set; } = Default.SuccessIcon;

    [Export] public Color InfoColor { get; set; } = Default.InfoColor;
    [Export] public Color WarnColor { get; set; } = Default.WarnColor;
    [Export] public Color ErrorColor { get; set; } = Default.ErrorColor;
    [Export] public Color SuccessColor { get; set; } = Default.SuccessColor;

    #endregion

    #region Utils

    public Color? Color(Status status) => status switch
    {
        Status.Info => InfoColor.NullIfDefault(),
        Status.Warn => WarnColor.NullIfDefault(),
        Status.Error => ErrorColor.NullIfDefault(),
        Status.Success => SuccessColor.NullIfDefault(),
        _ => null,
    };

    public Texture2D Texture(Status status) => status switch
    {
        Status.Info => InfoIcon,
        Status.Warn => WarnIcon,
        Status.Error => ErrorIcon,
        Status.Success => SuccessIcon,
        _ => null,
    };

    public Control Icon(Status status) => UI.TextureRect("Icon", Texture(status));
    public Control Text(Status status, string msg) => UI.SunkLabel("Text", msg, Color(status));

    public static StatusConfig New(string path, Color? cInfo = null, Color? cWarn = null, Color? cError = null, Color? cSuccess = null)
        => New(Status.Info.Load(path), Status.Warn.Load(path), Status.Error.Load(path), Status.Success.Load(path), cInfo, cWarn, cError, cSuccess);

    public static StatusConfig New(Texture2D tInfo = null, Texture2D tWarn = null, Texture2D tError = null, Texture2D tSuccess = null, Color? cInfo = null, Color? cWarn = null, Color? cError = null, Color? cSuccess = null) => new()
    {
        InfoIcon = tInfo,
        WarnIcon = tWarn,
        ErrorIcon = tError,
        SuccessIcon = tSuccess,

        InfoColor = cInfo ?? tInfo?.GetImage()?.MostCommonColor() ?? default,
        WarnColor = cWarn ?? tWarn?.GetImage()?.MostCommonColor() ?? default,
        ErrorColor = cError ?? tError?.GetImage()?.MostCommonColor() ?? default,
        SuccessColor = cSuccess ?? tSuccess?.GetImage()?.MostCommonColor() ?? default,
    };

    #endregion
}
