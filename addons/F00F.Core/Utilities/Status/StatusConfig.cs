using System;
using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class StatusConfig : CustomResource
{
    #region Defaults

    public static class Default
    {
        private static readonly string ResPath = Utils.GetResPath("Assets/Status{0}.svg");

        public static readonly Color Red = new("e74c3c");
        public static readonly Color Green = new("2ecc71");
        public static readonly Color Yellow = new("f1c40f");
        public static readonly Color DarkText = new("1a1a1a");

        public static readonly Color InfoColor = default;
        public static readonly Color WarnColor = /*WarnIcon?.GetImage()?.MostCommonColor() ??*/ Yellow;
        public static readonly Color ErrorColor = /*ErrorIcon?.GetImage()?.MostCommonColor() ??*/ Red;
        public static readonly Color SuccessColor = /*SuccessIcon?.GetImage()?.MostCommonColor() ??*/ Green;

        public static readonly Color InfoTextColor = default;
        public static readonly Color WarnTextColor = DarkText;
        public static readonly Color ErrorTextColor = DarkText;
        public static readonly Color SuccessTextColor = DarkText;

        public static Texture2D InfoIcon => field ??= default;
        public static Texture2D WarnIcon => field ??= Status.Warn.Load(ResPath);
        public static Texture2D ErrorIcon => field ??= Status.Error.Load(ResPath);
        public static Texture2D SuccessIcon => field ??= Status.Success.Load(ResPath);

        public static StatusConfig Instance => field ??= new();
    }

    #endregion

    #region Export

    [Export] public Color InfoColor { get; set; } = Default.InfoColor;
    [Export] public Color WarnColor { get; set; } = Default.WarnColor;
    [Export] public Color ErrorColor { get; set; } = Default.ErrorColor;
    [Export] public Color SuccessColor { get; set; } = Default.SuccessColor;

    [Export] public Color InfoTextColor { get; set; } = Default.InfoTextColor;
    [Export] public Color WarnTextColor { get; set; } = Default.WarnTextColor;
    [Export] public Color ErrorTextColor { get; set; } = Default.ErrorTextColor;
    [Export] public Color SuccessTextColor { get; set; } = Default.SuccessTextColor;

    [Export] public Texture2D InfoIcon { get; set; } = Default.InfoIcon;
    [Export] public Texture2D WarnIcon { get; set; } = Default.WarnIcon;
    [Export] public Texture2D ErrorIcon { get; set; } = Default.ErrorIcon;
    [Export] public Texture2D SuccessIcon { get; set; } = Default.SuccessIcon;

    #endregion

    #region Utils

    public Color Modulate(Status status) => Color(status) ?? Const.ModulateDflt;
    public Color? BackColor(Status status) => Color(status);
    public Color? Color(Status status) => status switch
    {
        Status.Info => InfoColor.NullIfDefault(),
        Status.Warn => WarnColor.NullIfDefault(),
        Status.Error => ErrorColor.NullIfDefault(),
        Status.Success => SuccessColor.NullIfDefault(),
        _ => null,
    };

    public Color? FontColor(Status status) => TextColor(status);
    public Color? ForeColor(Status status) => TextColor(status);
    public Color? TextColor(Status status) => status switch
    {
        Status.Info => InfoTextColor.NullIfDefault(),
        Status.Warn => WarnTextColor.NullIfDefault(),
        Status.Error => ErrorTextColor.NullIfDefault(),
        Status.Success => SuccessTextColor.NullIfDefault(),
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

    public TextureRect StatusIcon(Status status)
        => UI.TextureRect(nameof(StatusIcon), Texture(status));

    public LineEdit StatusLabel(Status status, string msg, bool SetBackColor = true)
    {
        return UI.SunkLabel(nameof(StatusLabel), msg, TextColor(), BackColor());

        Color? TextColor() => SetBackColor ? status.TextColor() : status.Color();
        Color? BackColor() => SetBackColor ? status.BackColor() : null;
    }

    public Button StatusButton(Status status, string msg, Action action, bool SetBackColor = true)
    {
        var btn = NewBtn();
        SetBtnColors();
        return btn;

        Button NewBtn()
            => UI.Button(nameof(StatusButton), action, msg, flat: true);

        void SetBtnColors()
        {
            if (SetBackColor)
            {
                btn.SetTextColor(TextColor(status));
                btn.SetBackColor(BackColor(status));
            }
            else
            {
                btn.SetTextColor(Color(status));
            }
        }
    }

    public static StatusConfig New(string path,
        Color? InfoColor = null, Color? WarnColor = null, Color? ErrorColor = null, Color? SuccessColor = null,
        Color? InfoTextColor = null, Color? WarnTextColor = null, Color? ErrorTextColor = null, Color? SuccessTextColor = null,
        bool UseMostCommonColor = false, bool UseNullDefaultColor = false)
        => New(Status.Info.Load(path), Status.Warn.Load(path), Status.Error.Load(path), Status.Success.Load(path),
            InfoColor, WarnColor, ErrorColor, SuccessColor, InfoTextColor, WarnTextColor, ErrorTextColor, SuccessTextColor,
            UseMostCommonColor, UseNullDefaultColor);

    public static StatusConfig New(
        Texture2D InfoIcon = null, Texture2D WarnIcon = null, Texture2D ErrorIcon = null, Texture2D SuccessIcon = null,
        Color? InfoColor = null, Color? WarnColor = null, Color? ErrorColor = null, Color? SuccessColor = null,
        Color? InfoTextColor = null, Color? WarnTextColor = null, Color? ErrorTextColor = null, Color? SuccessTextColor = null,
        bool UseMostCommonColor = false, bool UseNullDefaultColor = false)
    {
        return new()
        {
            InfoIcon = InfoIcon,
            WarnIcon = WarnIcon,
            ErrorIcon = ErrorIcon,
            SuccessIcon = SuccessIcon,

            InfoColor = InfoColor ?? IconColor(InfoIcon) ?? DefaultColor(default),
            WarnColor = WarnColor ?? IconColor(WarnIcon) ?? DefaultColor(Default.Yellow),
            ErrorColor = ErrorColor ?? IconColor(ErrorIcon) ?? DefaultColor(Default.Red),
            SuccessColor = SuccessColor ?? IconColor(SuccessIcon) ?? DefaultColor(Default.Green),

            InfoTextColor = InfoTextColor ?? DefaultColor(default),
            WarnTextColor = WarnTextColor ?? DefaultColor(Default.DarkText),
            ErrorTextColor = ErrorTextColor ?? DefaultColor(Default.DarkText),
            SuccessTextColor = SuccessTextColor ?? DefaultColor(Default.DarkText),
        };

        Color? IconColor(Texture2D icon)
            => UseMostCommonColor ? icon?.GetImage()?.MostCommonColor() : null;

        Color DefaultColor(in Color dflt)
            => UseNullDefaultColor ? default : dflt;
    }

    #endregion
}
