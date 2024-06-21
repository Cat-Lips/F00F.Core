using System;
using System.Collections.Generic;
using Godot;
using WatchTarget = (Godot.Label Label, Godot.Label Value, System.Func<string> GetValue, ulong? StartTime);

namespace F00F
{
    [Tool]
    public partial class ValueWatcher : DataView
    {
        private readonly DeltaTimer timer = new();
        private Dictionary<string, WatchTarget> WatchTargets { get; } = [];

        public static ValueWatcher Instance { get; private set; }

        public sealed override void _Ready()
        {
            Instance ??= this;
            OnReady();
        }

        protected virtual void OnReady() { }

        public void Add<T>(string name, Func<T> GetValue, bool timed = false) => Add(name, () => $"{GetValue()}", timed);
        public void Add(string name, Func<string> GetValue, bool timed = false)
        {
            Remove(name);

            var label = NewLabel($"{name}Label", name.Capitalize());
            var value = NewLabel($"{name}Value", align: HorizontalAlignment.Right).ExpandToFitText();

            Visible = true;
            Grid.AddChild(label, forceReadableName: true);
            Grid.AddChild(value, forceReadableName: true);

            WatchTargets.Add(name, (label, value, GetValue, timed ? Time.GetTicksMsec() : null));

            static Label NewLabel(string name, string text = null, HorizontalAlignment align = HorizontalAlignment.Left) => new()
            {
                Name = name,
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = align,
            };
        }

        public void Remove(string name, bool log = false) => Remove(name, log ? GD.Print : null);
        public void Remove(string name, Action<string> final)
        {
            if (WatchTargets.Remove(name, out var target))
            {
                Visible = WatchTargets.Count is not 0;
                Grid.RemoveChild(target.Label, free: true);
                Grid.RemoveChild(target.Value, free: true);
                final?.Invoke($"{target.Label.Text}: {target.Value.Text}");
            }
        }

        public override void _Process(double delta)
        {
            if (timer.Ready((float)delta))
            {
                foreach (var (label, value, GetValue, startTime) in WatchTargets.Values)
                    value.Text = Format(GetValue(), startTime);
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
