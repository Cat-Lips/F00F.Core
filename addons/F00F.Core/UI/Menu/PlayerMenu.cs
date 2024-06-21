using System;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerMenu : SlideMenu
{
    private LineEdit PlayerName => field ??= GetNode<LineEdit>("%PlayerName");
    private ColorPickerButton PlayerColor => field ??= GetNode<ColorPickerButton>("%PlayerColor");
    private OptionButton PlayerAvatar => field ??= GetNode<OptionButton>("%PlayerAvatar");

    public PlayerMenu()
        => Visible = Editor.IsEditor;

    protected sealed override void OnReady1()
    {
        PlayerName.Visible = Editor.IsEditor;
        PlayerColor.Visible = Editor.IsEditor;
        PlayerAvatar.Visible = Editor.IsEditor;
    }

    public void EnablePlayerName(Config cfg, Action<string> PlayerNameChanged, Action<string> PlayerNameEdited = null)
    {
        LoadPlayerName();
        EnablePlayerName();

        void LoadPlayerName()
            => PlayerName.Text = _LoadPlayerName();

        void EnablePlayerName()
        {
            Visible = true;
            PlayerName.Visible = true;
            PlayerName.LoseFocusOnEnter();
            PlayerName.TextChanged += x => PlayerNameEdited?.Invoke(x);
            PlayerName.FocusExited += OnPlayerNameChanged;
            OnPlayerNameChanged();

            void OnPlayerNameChanged()
            {
                var value = PlayerName.Text;
                if (!ValidPlayerName(value))
                    PlayerName.Text = value = _LoadPlayerName();
                _SavePlayerName(value);
                PlayerNameChanged?.Invoke(value);
            }

            bool ValidPlayerName(string value)
                => !string.IsNullOrWhiteSpace(value) && value != PlayerName.PlaceholderText;
        }

        void _SavePlayerName(string value)
            => cfg.Set(this, PlayerName, App.RemoveId(value));

        string _LoadPlayerName()
            => App.AddId(cfg.Get(this, PlayerName, _DefaultPlayerName()));

        static string _DefaultPlayerName()
            => System.Environment.UserName;
    }

    public void EnablePlayerColor(Config cfg, Action<Color> PlayerColorChanged)
    {
        LoadPlayerColor();
        EnablePlayerColor();

        void LoadPlayerColor()
            => PlayerColor.Color = cfg.Get(this, PlayerColor, Const.DefaultColors.PickRandom());

        void EnablePlayerColor()
        {
            Visible = true;
            PlayerColor.Visible = true;
            PlayerColor.ColorChanged += _ => OnColorSelected();
            OnColorSelected();

            void OnColorSelected()
            {
                var color = PlayerColor.Color;
                cfg.Set(this, PlayerColor, color);
                PlayerColorChanged?.Invoke(color);
            }
        }
    }

    public void EnablePlayerAvatar(Config cfg, Action<string> PlayerAvatarChanged, params string[] keys)
    {
        if (keys.NotEmpty())
        {
            PlayerAvatar.AddItems(keys);
            EnablePlayerAvatar(cfg, PlayerAvatarChanged, keys.PickRandom());
        }
    }

    public void EnablePlayerAvatar(Config cfg, Action<string> PlayerAvatarChanged, params (string Key, Texture2D Icon)[] items)
    {
        if (items.NotEmpty())
        {
            PlayerAvatar.AddItems(items);
            EnablePlayerAvatar(cfg, PlayerAvatarChanged, items.PickRandom().Key);
        }
    }

    private void EnablePlayerAvatar(Config cfg, Action<string> PlayerAvatarChanged, string @default)
    {
        LoadPlayerAvatar();
        EnablePlayerAvatar();

        void LoadPlayerAvatar()
        {
            var key = cfg.Get(this, PlayerAvatar, @default);
            PlayerAvatar.Selected = PlayerAvatar.IndexOf(key);
        }

        void EnablePlayerAvatar()
        {
            Visible = true;
            PlayerAvatar.Visible = true;
            PlayerAvatar.ItemSelected += _ => OnAvatarSelected();
            OnAvatarSelected();

            void OnAvatarSelected()
            {
                var key = (string)PlayerAvatar.GetSelectedMetadata();
                cfg.Set(this, PlayerAvatar, key);
                PlayerAvatarChanged?.Invoke(key);
            }
            ;
        }
    }
}
