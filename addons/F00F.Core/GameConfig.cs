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

    string[] PlayerData { get; }

    string[] PlayerAvatars { get; }
    (string Key, Texture2D Icon)[] AvatarIcons();

    // MainMenu1
    void AddGameOptions(PopupMenu root);
    (string Label, Action Action)[] GameOptions();

    // MainMenu2
    string[] GameOptionGroups { get; }
    (string Label, Control Control)[] GetGameOptions();
    (string Label, Control Control)[] GetGameOptions(string group);

    Key QuickHelpKey { get; }
    bool ShowQuickHelp { get; }
    string QuickHelpFile { get; }

    Key QuickExitKey { get; }
}

public class GameConfig : IGameConfig
{
    public virtual bool EnableQuitGame => OS.HasFeature("pc");
    public virtual bool EnableQuickHelp => FS.FileExists(QuickHelpFile);
    public virtual bool EnableGameOptions => true;

    public virtual bool EnableMultiplayer => true;
    public virtual bool EnableSinglePlayer => true;

    public virtual bool EnablePlayerName => true;
    public virtual bool EnablePlayerColor => true;
    public virtual bool EnablePlayerAvatars => true;

    public virtual string[] PlayerData => null;

    public virtual string[] PlayerAvatars => null;
    public virtual (string Key, Texture2D Icon)[] AvatarIcons() => null;

    // MainMenu1
    public virtual void AddGameOptions(PopupMenu root) { } // required for anything other than basic menu
    public virtual (string Label, Action Action)[] GameOptions() => null; // use null action for separator

    // MainMenu2
    public virtual string[] GameOptionGroups => null;
    public virtual (string Label, Control Control)[] GetGameOptions() => null;
    public virtual (string Label, Control Control)[] GetGameOptions(string group) => null;

    public virtual Key QuickHelpKey => Key.F1;
    public virtual bool ShowQuickHelp => _ShowQuickHelp();
    public virtual string QuickHelpFile => "res://Assets/help.txt";

    public virtual Key QuickExitKey => Key.End;

    private bool _ShowQuickHelp()
    {
        var cfg = new Config<GameConfig>();
        var showHelp = cfg.Get(nameof(ShowQuickHelp), true);
        cfg.Set(nameof(ShowQuickHelp), false);
        return showHelp;
    }
}

public sealed class TestConfig : GameConfig
{
    //public sealed override bool EnableQuitGame => false;
    //public sealed override bool EnableQuickHelp => false;

    //public sealed override Key QuickHelpKey => Key.F1;
    //public sealed override bool ShowQuickHelp => true;

    //public sealed override Key QuickExitKey => Key.End;
}
