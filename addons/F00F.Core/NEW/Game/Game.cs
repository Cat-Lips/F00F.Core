using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F.NEW;

[Tool]
public partial class Game : Node
{
    #region Private

    protected GUI UX => field ??= (GUI)GetNode("UI");
    protected MyInput IX => field ??= (MyInput)GetNode("Input");
    protected Network Network => field ??= (Network)GetNode("Network");
    private QuickHelp QuickHelp => field ??= (QuickHelp)UX.GetNode("QuickHelp");

    #endregion

    #region Export

    [Export] public GameInput Input { get; set => this.Set(ref field, value ?? new()); }
    [Export] public GameConfig Config { get; set => this.Set(ref field, value ?? new()); }

    #endregion

    #region Godot

    protected virtual void OnReady() { }
    protected virtual void InitInput() { }
    public sealed override void _Ready()
    {
        Input ??= new();
        Config ??= new();
        Editor.Disable(this);
        if (Editor.IsEditor)
        {
            OnReady();
            return;
        }

        ParseCmdLine();
        InitInput();
        InitQuit();
        OnReady();

        static void ParseCmdLine()
            => CmdLine.Parse(/*Network*/);

        void InitQuit()
            => this.InitQuit();
    }

    protected virtual bool OnUnhandledKeyInput(InputEventKey e) => false;
    public sealed override void _UnhandledKeyInput(InputEvent e)
    {
        if (e is InputEventKey key)
        {
            if (OnUnhandledKeyInput(key)) return;
            if (Config.QuickHelp && this.Handle(Input.QuickHelp(key), QuickHelp)) return;
            if (Config.QuickExit && this.Handle(Input.QuickExit(key), QuickExit)) return;

            void QuickHelp()
                => this.QuickHelp.Visible = !this.QuickHelp.Visible;

            void QuickExit()
                => this.NotifyQuit();
        }
    }

    protected virtual bool OnNotify(long what) => false;
    public override void _Notification(int what)
    {
        if (OnNotify(what)) return;
        if (this.OnQuitNotify(what)) return;
#if TOOLS
        if (Editor.OnSave(what, OnEditorSave)) return;
#endif
    }

    #endregion

    #region Config

    private readonly Config<GameConfig> cfg = new();

    public T Load<T>(string name, T dflt = default) => cfg.Get(name, dflt);
    public T LoadEnum<T>(string name, T dflt = default) where T : struct, Enum => cfg.GetEnum(name, dflt);
    public T[] LoadEnums<T>(string name, T[] dflt = default) where T : struct, Enum => cfg.GetEnums(name, dflt);

    public void Save<T>(string name, T value) => cfg.Set(name, value);
    public void SaveEnum<T>(string name, T value) where T : struct, Enum => cfg.SetEnum(name, value);
    public void SaveEnums<T>(string name, T[] values) where T : struct, Enum => cfg.SetEnums(name, values);

    public T Load<T>(T dflt, [CallerArgumentExpression(nameof(dflt))] string name = null) => cfg.Get(name, dflt);
    public T LoadEnum<T>(T dflt, [CallerArgumentExpression(nameof(dflt))] string name = null) where T : struct, Enum => cfg.GetEnum(name, dflt);
    public T[] LoadEnums<T>(T[] dflt, [CallerArgumentExpression(nameof(dflt))] string name = null) where T : struct, Enum => cfg.GetEnums(name, dflt);

    public void Save<T>(T value, [CallerArgumentExpression(nameof(value))] string name = null) => cfg.Set(name, value);
    public void SaveEnum<T>(T value, [CallerArgumentExpression(nameof(value))] string name = null) where T : struct, Enum => cfg.SetEnum(name, value);
    public void SaveEnums<T>(T[] values, [CallerArgumentExpression(nameof(values))] string name = null) where T : struct, Enum => cfg.SetEnums(name, values);

    #endregion
}
