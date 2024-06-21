using System;
using Godot;

namespace F00F;

public interface IGameConfig
{
    bool EnableQuitGame { get; }
    bool EnableQuickHelp { get; }
    bool EnableGameOptions { get; }

    bool EnableMultiplayer { get; }
    bool EnableSinglePlayer { get; }

    bool EnablePlayerName { get; }
    bool EnablePlayerColor { get; }
    bool EnablePlayerAvatars { get; }

    string[] PlayerAvatars { get; }
    (string Key, Texture2D Icon)[] AvatarIcons();

    // MainMenu1
    void AddGameOptions(PopupMenu root);
    (string Label, Action Action)[] GameOptions();

    // MainMenu2
    string[] GameOptionGroups { get; }
    (string Label, Control Control)[] GetGameOptions();
    (string Label, Control Control)[] GetGameOptions(string group);

    string QuickHelp { get; }
    string[] PlayerData { get; }
}

internal class GameConfig : IGameConfig
{
    public virtual bool EnableQuitGame => OS.HasFeature("pc");
    public virtual bool EnableQuickHelp => FS.FileExists(QuickHelp);
    public virtual bool EnableGameOptions => true;

    public virtual bool EnableMultiplayer => true;
    public virtual bool EnableSinglePlayer => true;

    public virtual bool EnablePlayerName => true;
    public virtual bool EnablePlayerColor => true;
    public virtual bool EnablePlayerAvatars => true;

    public virtual string[] PlayerAvatars => null;
    public virtual (string Key, Texture2D Icon)[] AvatarIcons() => null;

    // MainMenu1
    public virtual void AddGameOptions(PopupMenu root) { } // required for anything other than basic menu
    public virtual (string Label, Action Action)[] GameOptions() => null; // null action for separator

    // MainMenu2
    public virtual string[] GameOptionGroups => null;
    public virtual (string Label, Control Control)[] GetGameOptions() => null;
    public virtual (string Label, Control Control)[] GetGameOptions(string group) => null;

    public virtual string QuickHelp => "res://Assets/Help/QuickHelp.txt";
    public virtual string[] PlayerData => null;
}
