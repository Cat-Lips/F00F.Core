using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using WatchTarget = (Godot.Control Label, Godot.Control Value, System.Func<string> GetValue, ulong? StartTime);

namespace F00F
{
    [Tool]
    public partial class ValueWatcher : DataView
    {
        private readonly DeltaTimer timer = new();
        private Dictionary<string, WatchTarget> WatchTargets { get; } = [];

        public static ValueWatcher Instance { get; private set; }

        public ValueWatcher()
        {
            if (!this.FakeEditorCtor())
                Instance = this;
        }

        protected virtual void OnReady() { }
        public sealed override void _Ready()
        {
            OnReady();
            Editor.Disable(this);
        }

        public void Add(StringName name, Func<string> GetValue = null, bool timed = false, bool capitalise = false, [CallerFilePath] string group = null) => Add(group.GetFileBaseName(), name.ToString(), GetValue, timed, capitalise);
        public void Add<T>(StringName name, Func<T> GetValue, bool timed = false, bool capitalise = false, [CallerFilePath] string group = null) => Add(group.GetFileBaseName(), name.ToString(), GetValue, timed, capitalise);
        public void Add<T>(string group, StringName name, Func<T> GetValue, bool timed = false, bool capitalise = false) => Add(group, name.ToString(), () => GetValue().ToString(), timed, capitalise);
        public void Add(string group, StringName name, Func<string> GetValue = null, bool timed = false, bool capitalise = false) => Add(group, name.ToString(), GetValue, timed, capitalise);

        public void Add(string name, Func<string> GetValue = null, bool timed = false, bool capitalise = true, [CallerFilePath] string group = null) => Add(group.GetFileBaseName(), name, GetValue, timed, capitalise);
        public void Add<T>(string name, Func<T> GetValue, bool timed = false, bool capitalise = true, [CallerFilePath] string group = null) => Add(group.GetFileBaseName(), name, GetValue, timed, capitalise);
        public void Add<T>(string group, string name, Func<T> GetValue, bool timed = false, bool capitalise = true) => Add(group, name, () => GetValue().ToString(), timed, capitalise);
        public void Add(string group, string name, Func<string> GetValue = null, bool timed = false, bool capitalise = true)
        {
            Remove(group, name);

            var key = $"{group}.{name}";
            GetControls(out var label, out var value);

            Visible = true;
            Grid.AddChild(label, forceReadableName: true);
            Grid.AddChild(value, forceReadableName: true);

            WatchTargets.Add(key, (label, value, GetValue, timed ? Time.GetTicksMsec() : null));

            void GetControls(out Control label, out Control value)
            {
                if (GetValue is null)
                {
                    label = NewSep($"{key}Label");
                    value = NewSep($"{key}Value");
                }
                else
                {
                    label = NewLabel($"{key}Label", capitalise ? name.Capitalise() : name);
                    value = NewLabel($"{key}Value", align: HorizontalAlignment.Right).ExpandToFitText();
                }

                static Label NewLabel(string name, string text = null, HorizontalAlignment align = HorizontalAlignment.Left) => new()
                {
                    Name = name,
                    Text = text,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = align,
                };

                static HSeparator NewSep(string name) => new()
                {
                    Name = name,
                };
            }
        }

        public void Remove(string name, bool log = false, [CallerFilePath] string group = null) => Remove(group.GetFileBaseName(), name, log ? GD.Print : null);
        public void Remove(string group, string name, bool log = false) => Remove(group, name, log ? GD.Print : null);
        public void Remove(string group, string name, Action<string> final)
        {
            var key = $"{group}.{name}";
            if (WatchTargets.Remove(key, out var target))
            {
                Visible = WatchTargets.Count is not 0;
                Grid.RemoveChild(target.Label, free: true);
                Grid.RemoveChild(target.Value, free: true);
                if (target.Label is Label label && target.Value is Label value)
                    final?.Invoke($"{label.Text}: {value.Text}");
            }
        }

        public void RemoveLocal([CallerFilePath] string group = null)
        {
            group = group.GetFileBaseName();
            WatchTargets.Keys
                .Where(key => key.StartsWith(group))
                .Select(key => key.Split('.', 2).Last())
                .ForEach(name => Remove(group, name));
        }

        public override void _Process(double delta)
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
    }
}
