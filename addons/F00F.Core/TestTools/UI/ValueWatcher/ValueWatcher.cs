using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using WatchList = Dictionary<string, (Control Label, Control Value, Func<string> GetValue, ulong? StartTime)>;

[Tool]
public partial class ValueWatcher : DataView
{
    #region Instance

    public static ValueWatcher Instance { get; private set; }

    public ValueWatcher()
    {
        if (this.FakeEditorCtor()) return;

        (Instance = this).OnReady(static () =>
        {
            Instance.SetProcess(Instance.IsVisibleInTree());
            Instance.VisibleInTreeChanged += () => Instance.SetProcess(Instance.IsVisibleInTree());
        });
    }

    #endregion

    #region Private

    protected const float RefreshRate = 1;
    private readonly DeltaTimer timer = new(RefreshRate);

    private WatchList Separators { get; } = [];
    private WatchList WatchTargets { get; } = [];
    private WatchList UrgentWatchTargets { get; } = [];

    #endregion

    [Export] public bool AutoShow { get; set; }

    public void Sep(string title, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Sep(GRP(f, n), title);

    public void Add(string name, Func<string> GetValue, bool timed = false, bool urgent = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, GetValue, timed, urgent);

    public void Add<T>(string name, Func<T> GetValue, bool timed = false, bool urgent = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, () => $"{GetValue()}", timed, urgent);

    public void Clear([CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Clear(GRP(f, n));

    public void Remove(string name, bool log = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Remove(GRP(f, n), name, log ? GD.Print : null);

    public void Remove(string name, Action<string> log, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Remove(GRP(f, n), name, log);

    #region Godot

    protected virtual void OnProcess(float delta) { }
    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;

        OnProcess(delta);
        if (timer.Ready(delta))
            Process(WatchTargets);
        Process(UrgentWatchTargets);

        void Process(WatchList source)
        {
            foreach (var (_, value, GetValue, startTime) in source.Values)
            {
                if (value is Label v)
                    v.Text = Format(GetValue(), startTime);
            }
        }

        static string Format(string source, ulong? startTime)
        {
            return startTime is null ? source : $"{source} ({TimeStr()})";

            string TimeStr()
                => Utils.HumaniseTime(Time.GetTicksMsec() - startTime.Value);
        }
    }

    #endregion

    #region Private

    private static string GRP([CallerFilePath] string CallerFilePath = null, [CallerMemberName] string CallerMemberName = null)
        => $"{CallerFilePath.GetFileBaseName()}.{CallerMemberName}";

    private static string Key(string group, string name) => $"{group}.{name}";

    private void _Sep(string group, string title)
    {
        var key = Key(group, title);
        var label = UI.Label($"{key}.Label", title);
        var value = UI.Sep($"{key}.Value");

        Grid.AddChild(label, forceReadableName: true);
        Grid.AddChild(value, forceReadableName: true);

        Separators.Add(key, (label, value, null, null));
    }

    private void _Add(string group, string name, Func<string> GetValue, bool timed, bool urgent)
    {
        if (AutoShow)
            Visible = true;

        var key = Key(group, name);
        var label = UI.Label($"{key}.Label", name);
        var value = UI.Label($"{key}.Value", "", align: HorizontalAlignment.Right).ExpandToFitWidth();

        Grid.AddChild(label, forceReadableName: true);
        Grid.AddChild(value, forceReadableName: true);

        (urgent ? UrgentWatchTargets : WatchTargets).Add(key, (label, value, GetValue, timed ? Time.GetTicksMsec() : null));
    }

    private void _Clear(string group)
    {
        var prefix = $"{group}.";

        Clear(Separators);
        Clear(WatchTargets);
        Clear(UrgentWatchTargets);

        void Clear(WatchList source)
        {
            source.Keys
                .Where(key => key.StartsWith(prefix))
                .Select(key => key.Split('.', 3).Last())
                .ForEach(name => _Remove(source, group, name));
        }
    }

    private void _Remove(string group, string name, Action<string> log = null)
    {
        _Remove(Separators, group, name, log);
        _Remove(WatchTargets, group, name, log);
        _Remove(UrgentWatchTargets, group, name, log);
    }

    private void _Remove(WatchList source, string group, string name, Action<string> log = null)
    {
        var key = Key(group, name);
        if (source.Remove(key, out var target))
        {
            Grid.RemoveChild(target.Label, free: true);
            Grid.RemoveChild(target.Value, free: true);
            if (target.Label is Label label && target.Value is Label value)
                log?.Invoke($"{label.Text}: {value.Text}");
            if (AutoShow && Grid.GetChildCount() is 0)
                Visible = false;
        }
    }

    #endregion
}
