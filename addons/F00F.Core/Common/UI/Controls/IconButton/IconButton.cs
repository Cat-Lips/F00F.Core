using Godot;

namespace F00F;

[Tool]
public partial class IconButton : Button
{
    #region Export

    [Export] public Texture2D HoverIcon { get; set => this.Set(ref field, value, SetIcon); }
    [Export] public Texture2D NormalIcon { get; set => this.Set(ref field, value, SetIcon); }
    [Export] public Texture2D PressedIcon { get; set => this.Set(ref field, value, SetIcon); }
    [Export] public Texture2D DisabledIcon { get; set => this.Set(ref field, value, SetIcon); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        SetIcon();

        ButtonUp += () => _BtnPressed = false;
        ButtonDown += () => _BtnPressed = true;
        MouseExited += () => _MouseOver = false;
        MouseEntered += () => _MouseOver = true;

        SetMinWidth();
        Resized += SetMinWidth;

        void SetMinWidth()
            => CustomMinimumSize = new(Size.Y, 0);
    }

    public sealed override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationEnabled:
            case NotificationDisabled:
                SetIcon();
                break;
        }
#if TOOLS
        if (Editor.OnPreSave(what))
        {
            if (this.IsEditedSceneRoot())
            {
                Editor.DoPreSaveResetField(this, PropertyName.HoverIcon);
                Editor.DoPreSaveResetField(this, PropertyName.NormalIcon);
                Editor.DoPreSaveResetField(this, PropertyName.PressedIcon);
                Editor.DoPreSaveResetField(this, PropertyName.DisabledIcon);
            }

            Editor.DoPreSaveReset(this, Button.PropertyName.Icon);
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
#endif
    }

    #endregion

    #region Private

    private bool _MouseOver { get; set => this.Set(ref field, value, SetIcon); }
    private bool _BtnPressed { get; set => this.Set(ref field, value, SetIcon); }

    private void SetIcon()
    {
        Icon = Disabled ? DisabledIcon ?? NormalIcon
            : _BtnPressed ? PressedIcon ?? NormalIcon
            : _MouseOver ? HoverIcon ?? NormalIcon
            : NormalIcon;
    }

    #endregion
}
