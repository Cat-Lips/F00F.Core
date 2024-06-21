using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using WatchList = Dictionary<string, (Control Label, Control Layout, Control Value, Func<string> GetValue, ulong? StartTime)>;

[Tool]
public partial class ValueWatcher : DataView
{
    public const int DefaultRound = 2;

    [Export] public bool AutoShow { get; set; }

    #region Instance

    public static ValueWatcher Instance { get => field.ValidOrNull(); private set; }
    public ValueWatcher() => Instance ??= this;

    public static void InitDebug(Node source, Action InitDebug, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
    {
        if (Editor.IsEditor) return;
        if (Instance.IsNull()) return;
        source.SafeInit(InitDebug, RemoveDebug);

        void RemoveDebug()
            => Instance?.Clear(f, n);
    }

    #endregion

    #region Private

    private readonly DeltaTimer timer = new();

    private WatchList Separators { get; } = [];
    private WatchList WatchTargets { get; } = [];
    private WatchList UrgentWatchTargets { get; } = [];

    #endregion

    public void Sep(string title, Control cfg = null, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Sep(GRP(f, n), title, cfg);

    public void Add(string name, Func<string> GetValue, Control cfg = null, bool timed = false, bool urgent = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, GetValue, cfg, timed, urgent);

    public void Add<T>(string name, Func<T> GetValue, Control cfg = null, bool timed = false, bool urgent = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, () => $"{GetValue()}", cfg, timed, urgent);

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

    public bool Has(string name, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Has(GRP(f, n), name);

    #region Godot

    protected virtual void InitItems() { }
    protected sealed override void OnReady()
    {
        Activate();
        Instance.VisibleInTreeChanged += Activate;

        InitItems();

        static void Activate()
            => Instance.SetProcess(Instance.IsVisibleInTree());
    }

    protected virtual void Process(bool all) { }
    public sealed override void _Process(double _delta)
    {
        var delta = (float)_delta;
        var all = timer.Ready(delta);
        Process(all);
        Update(all);
    }

    protected void Update(bool all)
    {
        if (all) Update(WatchTargets);
        Update(UrgentWatchTargets);

        void Update(WatchList source)
        {
            foreach (var (_, _, value, GetValue, startTime) in source.Values)
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
        => $"{CallerFilePath.GetFolderName()}.{CallerFilePath.GetFileName()}.{CallerMemberName}";

    private static string Key(string group, string name) => $"{group}.{name}";

    private void _Sep(string group, string title, Control cfg)
    {
        var key = Key(group, title);
        var label = UI.Label($"{key}.Label", title);
        var value = (Control)UI.Sep($"{key}.Value");
        var layout = cfg.NotNull() ? UI.Layout($"{key}.Layout", value, cfg) : value;

        Grid.AddChild(label, forceReadableName: true);
        Grid.AddChild(layout, forceReadableName: true);

        Separators.Add(key, (label, layout, value, null, null));
    }

    private void _Add(string group, string name, Func<string> GetValue, Control cfg, bool timed, bool urgent)
    {
        if (AutoShow)
            Visible = true;

        var key = Key(group, name);
        var label = UI.Label($"{key}.Label", name);
        var value = (Control)UI.Label($"{key}.Value", "", align: HorizontalAlignment.Right).ExpandToFitWidth();
        var layout = cfg.NotNull() ? UI.Layout($"{key}.Layout", value, cfg) : value;

        Grid.AddChild(label, forceReadableName: true);
        Grid.AddChild(layout, forceReadableName: true);

        (urgent ? UrgentWatchTargets : WatchTargets).Add(key, (label, layout, value, GetValue, timed ? Time.GetTicksMsec() : null));
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
            Grid.RemoveChild(target.Layout, free: true);
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

    private bool _Has(string group, string name)
    {
        var key = Key(group, name);
        return Has(Separators) || Has(WatchTargets) || Has(UrgentWatchTargets);

        bool Has(WatchList source)
            => source.ContainsKey(key);
    }

    #endregion
}
