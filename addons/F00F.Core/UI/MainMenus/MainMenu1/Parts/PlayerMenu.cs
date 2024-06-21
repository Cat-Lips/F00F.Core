using System;
using Godot;

namespace F00F;

[Tool]
public partial class PlayerMenu : SlideMenu
{
    private LineEdit PlayerName => field ??= (LineEdit)GetNode("%PlayerName");
    private ColorPickerButton PlayerColor => field ??= (ColorPickerButton)GetNode("%PlayerColor");
    private OptionButton PlayerAvatar => field ??= (OptionButton)GetNode("%PlayerAvatar");

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
        {
            var name = _LoadPlayerName();
            PlayerName.Text = name;
            PlayerNameChanged?.Invoke(name);
        }

        void EnablePlayerName()
        {
            Visible = true;
            PlayerName.Visible = true;
            PlayerName.LoseFocusOnEnter();
            PlayerName.TextChanged += x => PlayerNameEdited?.Invoke(x);
            PlayerName.FocusExited += OnPlayerNameChanged;

            void OnPlayerNameChanged()
            {
                var name = PlayerName.Text;
                if (!ValidPlayerName(name))
                    PlayerName.Text = name = _LoadPlayerName();
                _SavePlayerName(name);
                PlayerNameChanged?.Invoke(name);
            }

            bool ValidPlayerName(string name)
                => !string.IsNullOrWhiteSpace(name) && name != PlayerName.PlaceholderText;
        }

        void _SavePlayerName(string name)
            => cfg.Set(this, PlayerName, App.RemoveId(name));

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
        if (!keys.IsNullOrEmpty())
        {
            PlayerAvatar.AddItems(keys);
            EnablePlayerAvatar(cfg, PlayerAvatarChanged, keys.PickRandom());
        }
    }

    public void EnablePlayerAvatar(Config cfg, Action<string> PlayerAvatarChanged, params (string Key, Texture2D Icon)[] items)
    {
        if (!items.IsNullOrEmpty())
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
        }
    }
}
