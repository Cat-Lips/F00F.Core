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
    [Export] public bool AutoShow { get; set; }

    #region Instance

    public static ValueWatcher Instance { get => field.ValidOrNull(); private set; }
    public ValueWatcher() => Instance ??= this;

    #endregion

    #region Private

    private readonly DeltaTimer timer = new();

    private WatchList Separators { get; } = [];
    private WatchList WatchTargets { get; } = [];
    private WatchList UrgentWatchTargets { get; } = [];

    #endregion

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

    public void Show(string name, bool show = true, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Show(GRP(f, n), name, show);

    public void Hide(string name, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Show(GRP(f, n), name, false);

    #region Godot

    protected virtual void Initialise() { }
    protected sealed override void OnReady()
    {
        Instance.SetProcess(Instance.IsVisibleInTree());
        Instance.VisibleInTreeChanged += () => Instance.SetProcess(Instance.IsVisibleInTree());

        Initialise();
    }

    protected virtual void OnProcess(float delta) { }
    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;

        OnProcess(delta);
        Update(timer.Ready(delta));
    }

    protected void Update(bool all)
    {
        if (all) Update(WatchTargets);
        Update(UrgentWatchTargets);

        void Update(WatchList source)
        {
            foreach (var (_, value, GetValue, startTime) in source.Values)
            {
                if (value is Label v && v.Visible)
                    v.Text = Format(GetValue(), startTime);
            }

            static string Format(string source, ulong? startTime)
            {
                return startTime is null ? source : $"{source} ({TimeStr()})";

                string TimeStr()
                    => Utils.HumaniseTime(Time.GetTicksMsec() - startTime.Value);
            }
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
                .Select(key => key.TrimPrefix(prefix))
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

    private void _Show(string group, string name, bool show)
    {
        var key = Key(group, name);
        Show(Separators);
        Show(WatchTargets);
        Show(UrgentWatchTargets);

        void Show(WatchList source)
        {
            if (source.TryGetValue(key, out var item))
            {
                item.Label.Visible = show;
                item.Value.Visible = show;
            }
        }
    }

    #endregion
}
