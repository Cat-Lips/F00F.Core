using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ControlPair = (Godot.Control Label, Godot.Control EditControl);

namespace F00F;

public static partial class UI
{
    public static void Init(this GridContainer grid, IEnumerable<ControlPair> content)
    {
        content.ForEach(pair =>
        {
            grid.AddChild(pair.Label, forceReadableName: true);
            grid.AddChild(pair.EditControl, forceReadableName: true);
        });
    }

    public static void AddRowSeparator(this GridContainer grid)
    {
        var count = grid.Columns;
        for (var i = 0; i < count; ++i)
            grid.AddChild(NewSep(), forceReadableName: true);
    }

    public static string[] Items<T>() where T : struct, Enum
        => Enum.GetNames<T>().ToArray();

    public static string[] Items<T>(params T[] enums) where T : struct, Enum
        => enums.Select(x => $"{x}").ToArray();

    public static (string Name, int Id)[] ItemIds<T>() where T : struct, Enum
        => Enum.GetNames<T>().Zip(Enum.GetValues<T>().Select(x => (int)(object)x)).ToArray();

    public static (string Name, int Id)[] ItemIds<T>(params T[] enums) where T : struct, Enum
        => enums.Select(x => ($"{x}", (int)(object)x)).ToArray();

    public static int[] AddItems(this MenuButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddItems(items);
    public static int[] AddItems(this OptionButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddItems(items);
    public static int[] AddCheckItems(this MenuButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddCheckItems(items);
    public static int[] AddCheckItems(this OptionButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddCheckItems(items);
    public static int[] AddRadioItems(this MenuButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddRadioItems(items);
    public static int[] AddRadioItems(this OptionButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddRadioItems(items);

    public static int[] AddItems(this PopupMenu source, params IEnumerable<(string Label, Action Action)> items) => source.AddItems(Check.None, items);
    public static int[] AddCheckItems(this PopupMenu source, params IEnumerable<(string Label, Action Action)> items) => source.AddItems(Check.Check, items);
    public static int[] AddRadioItems(this PopupMenu source, params IEnumerable<(string Label, Action Action)> items) => source.AddItems(Check.Radio, items);

    private enum Check { None, Check, Radio }
    private static int[] AddItems(this PopupMenu source, Check style, params IEnumerable<(string Label, Action Action)> items)
    {
        var lookup = new Dictionary<long, Action>();
        source.IndexPressed += x => lookup.TryGet(x)?.Invoke();
        if (style is Check.Radio) source.IndexPressed += x => lookup.Keys.ForEach(y => source.SetItemChecked((int)y, x == y));

        var idx = source.ItemCount;
        foreach (var (label, action) in items)
        {
            switch (style)
            {
                case Check.None: source.AddItem(label); break;
                case Check.Check: source.AddCheckItem(label); break;
                case Check.Radio: source.AddRadioCheckItem(label); break;
                default: throw new NotImplementedException($"{style}");
            }

            lookup.Add(idx++, action);
        }

        return lookup.Keys.Select(x => (int)x).ToArray();
    }

    public static void EnableItems<T>(this MenuButton source, params T[] items) where T : struct, Enum => source.GetPopup().EnableItems(items);
    public static void EnableItems<T>(this OptionButton source, params T[] items) where T : struct, Enum => source.GetPopup().EnableItems(items);
    public static PopupMenu EnableItems<T>(this PopupMenu source, params T[] items) where T : struct, Enum
    {
        var count = source.ItemCount;
        var lookup = new HashSet<T>(items ?? []);
        for (var i = 0; i < count; ++i)
            source.SetItemDisabled(i, !lookup.Contains((T)(object)source.GetItemId(i)));
        return source;
    }

    public static T ExpandToFitText<T>(this T source) where T : Label
    {
        source.Resized += PreventShrink;
        return source;

        void PreventShrink()
        {
            var newWidth = source.Size.X;
            var curWidth = source.CustomMinimumSize.X;
            var maxWidth = Math.Max(newWidth, curWidth);
            source.CustomMinimumSize = new(maxWidth, default);
        }
    }

    public static T LoseFocusOnEnter<T>(this T source) where T : LineEdit
    {
        source.GuiInput += OnInput;
        return source;

        void OnInput(InputEvent e)
        {
            if (e is InputEventKey key && key.Keycode is Key.Enter or Key.KpEnter)
                source.ReleaseFocus();
        }
    }

    public static T SetSquareLayout<T>(this T source) where T : Control
    {
        source.Resized += OnSizeChanged;
        return source;

        void OnSizeChanged()
        {
            var minSize = Math.Max(source.Size.X, source.Size.Y);
            source.CustomMinimumSize = new(minSize, minSize);
        }
    }

    public static T InitPopup<T>(this T source) where T : Window
    {
        source.ForEachChild<Control>(x => x.MinimumSizeChanged += ResetMinSize);
        source.CloseRequested += source.Hide;
        return source;

        void ResetMinSize()
            => source.MinSize = (Vector2I)source.GetContentsMinimumSize();
    }
}
