using System;
using Godot;

namespace F00F;

public interface IGameConfig
{
    bool EnableQuitGame { get; }
    bool EnableQuickExit { get; }
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

    Key QuickExitKey { get; }
    string QuickHelp { get; }
    string[] PlayerData { get; }
}

public class GameConfig : IGameConfig
{
    public virtual bool EnableQuitGame => !EnableQuickExit && OS.HasFeature("pc");
    public virtual bool EnableQuickExit => QuickExitKey is not Key.None;
    public virtual bool EnableQuickHelp => FS.ResExists(QuickHelp);
    public virtual bool EnableGameOptions => false;

    public virtual bool EnableMultiplayer => false;
    public virtual bool EnableSinglePlayer => false;

    public virtual bool EnablePlayerName => false;
    public virtual bool EnablePlayerColor => false;
    public virtual bool EnablePlayerAvatars => false;

    public virtual string[] PlayerAvatars => null;
    public virtual (string Key, Texture2D Icon)[] AvatarIcons() => null;

    // MainMenu1
    public virtual void AddGameOptions(PopupMenu root) { } // required for anything other than basic menu
    public virtual (string Label, Action Action)[] GameOptions() => null; // null action for separator

    // MainMenu2
    public virtual string[] GameOptionGroups => null;
    public virtual (string Label, Control Control)[] GetGameOptions() => null;
    public virtual (string Label, Control Control)[] GetGameOptions(string group) => null;

    public virtual Key QuickExitKey => Key.None;
    public virtual string QuickHelp => "res://Assets/help.txt";
    public virtual string[] PlayerData => null;
}

public sealed class TestConfig : GameConfig
{
    public sealed override Key QuickExitKey => Key.End;
}
