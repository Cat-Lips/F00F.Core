using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

using WatchTarget = (Control Label, Control Value, Func<string> GetValue, ulong? StartTime);

[Tool]
public partial class ValueWatcher : DataView
{
    #region Instance

    public static ValueWatcher Instance { get; private set; }
    public ValueWatcher() { if (!this.FakeEditorCtor()) Instance = this; }

    #endregion

    #region Private

    private readonly DeltaTimer timer = new();
    private Dictionary<string, WatchTarget> WatchTargets { get; } = [];

    #endregion

    public void Sep(string title, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Sep(GRP(f, n), title);

    public void Add(string name, Func<string> GetValue, bool timed = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, GetValue, timed);

    public void Add<T>(string name, Func<T> GetValue, bool timed = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Add(GRP(f, n), name, () => $"{GetValue()}", timed);

    public void Clear([CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Clear(GRP(f, n));

    public void Remove(string name, bool log = false, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Remove(GRP(f, n), name, log ? GD.Print : null);

    public void Remove(string name, Action<string> log, [CallerFilePath] string f = null, [CallerMemberName] string n = null)
        => _Remove(GRP(f, n), name, log);

    #region Godot

    public sealed override void _Process(double delta)
    {
        if (timer.Ready((float)delta))
        {
            foreach (var (_, value, GetValue, startTime) in WatchTargets.Values)
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

        WatchTargets.Add(key, (label, value, null, null));
    }

    private void _Add(string group, string name, Func<string> GetValue, bool timed)
    {
        Visible = true;

        var key = Key(group, name);
        var label = UI.Label($"{key}.Label", name);
        var value = UI.Label($"{key}.Value", align: HorizontalAlignment.Right).ExpandToFitWidth();

        Grid.AddChild(label, forceReadableName: true);
        Grid.AddChild(value, forceReadableName: true);

        WatchTargets.Add(key, (label, value, GetValue, timed ? Time.GetTicksMsec() : null));
    }

    private void _Clear(string group)
    {
        WatchTargets.Keys
            .Where(key => key.StartsWith(group))
            .Select(key => key.Split('.', 3).Last())
            .ForEach(name => _Remove(group, name));
    }

    private void _Remove(string group, string name, Action<string> log = null)
    {
        var key = Key(group, name);
        if (WatchTargets.Remove(key, out var target))
        {
            Visible = WatchTargets.NotEmpty();
            Grid.RemoveChild(target.Label, free: true);
            Grid.RemoveChild(target.Value, free: true);
            if (target.Label is Label label && target.Value is Label value)
                log?.Invoke($"{label.Text}: {value.Text}");
        }
    }

    #endregion
}
