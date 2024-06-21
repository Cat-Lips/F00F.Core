using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

using ControlPair = (Control Label, Control EditControl);

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
        => [.. Enum.GetNames<T>()];

    public static string[] Items<T>(params T[] enums) where T : struct, Enum
        => [.. enums.Select(x => $"{x}")];

    public static (string Name, int Id)[] ItemIds<T>() where T : struct, Enum
        => [.. Enum.GetNames<T>().Zip(Enum.GetValues<T>().Select(x => Convert.ToInt32(x)))];

    public static (string Name, int Id)[] ItemIds<T>(params T[] enums) where T : struct, Enum
        => [.. enums.Select(x => ($"{x}", Convert.ToInt32(x)))];

    public static int[] AddItems(this MenuButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddItems(items);
    public static int[] AddItems(this OptionButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddItems(items);
    public static int[] AddItems(this PopupMenu source, params IEnumerable<(string Label, Action Action)> items)
    {
        var lookup = new Dictionary<long, Action>();
        source.IndexPressed += x => lookup.TryGet(x)?.Invoke();

        var idx = source.ItemCount - 1;
        foreach (var (label, action) in items)
        {
            ++idx;

            if (action is null)
            {
                source.AddSeparator(label);
                continue;
            }

            source.AddItem(label);
            lookup.Add(idx, action);
        }

        return [.. lookup.Keys.Select(x => (int)x)];
    }

    public static int[] AddCheckItems(this MenuButton source, params IEnumerable<(string Label, Action<bool> Action)> items) => source.GetPopup().AddCheckItems(items);
    public static int[] AddCheckItems(this OptionButton source, params IEnumerable<(string Label, Action<bool> Action)> items) => source.GetPopup().AddCheckItems(items);
    public static int[] AddCheckItems(this PopupMenu source, params IEnumerable<(string Label, Action<bool> Action)> items)
    {
        var lookup = new Dictionary<long, Action<bool>>();
        source.IndexPressed += x => lookup.TryGet(x)?.Invoke(source.IsItemChecked((int)x));

        var idx = source.ItemCount - 1;
        foreach (var (label, action) in items)
        {
            ++idx;

            if (action is null)
            {
                source.AddSeparator(label);
                continue;
            }

            source.AddItem(label);
            lookup.Add(idx, action);
        }

        return [.. lookup.Keys.Select(x => (int)x)];
    }

    public static int[] AddRadioItems(this MenuButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddRadioItems(items);
    public static int[] AddRadioItems(this OptionButton source, params IEnumerable<(string Label, Action Action)> items) => source.GetPopup().AddRadioItems(items);
    public static int[] AddRadioItems(this PopupMenu source, params IEnumerable<(string Label, Action Action)> items)
    {
        int[] keys = null;
        var lookup = new Dictionary<long, Action>();
        source.IndexPressed += x =>
        {
            source.CheckRadioItem(keys, (int)x);
            lookup.TryGet(x)?.Invoke();
        };

        var idx = source.ItemCount - 1;
        foreach (var (label, action) in items)
        {
            ++idx;

            if (action is null)
            {
                source.AddSeparator(label);
                continue;
            }

            source.AddRadioCheckItem(label);
            lookup.Add(idx, action);
        }

        return keys = [.. lookup.Keys.Select(x => (int)x)];
    }

    public static void CheckRadioItem(this MenuButton source, int[] items, int idx) => source.GetPopup().CheckRadioItem(items, idx);
    public static void CheckRadioItem(this OptionButton source, int[] items, int idx) => source.GetPopup().CheckRadioItem(items, idx);
    public static void CheckRadioItem(this PopupMenu source, int[] items, int idx)
        => items.ForEach(x => source.SetItemChecked(idx, x == idx));

    public static void EnableItems(this MenuButton source, int[] items, bool enable = true) => source.GetPopup().EnableItems(items, enable);
    public static void EnableItems(this OptionButton source, int[] items, bool enable = true) => source.GetPopup().EnableItems(items, enable);
    public static void EnableItems(this PopupMenu source, int[] items, bool enable = true)
        => items.ForEach(idx => source.SetItemDisabled(idx, !enable));

    public static void DisableItems(this MenuButton source, int[] items, bool disable = true) => source.GetPopup().DisableItems(items, disable);
    public static void DisableItems(this OptionButton source, int[] items, bool disable = true) => source.GetPopup().DisableItems(items, disable);
    public static void DisableItems(this PopupMenu source, int[] items, bool disable = true)
        => items.ForEach(idx => source.SetItemDisabled(idx, disable));

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

    public static T ExpandToFitWidth<T>(this T source) where T : Control
    {
        PreventShrink();
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

    public static void ResetWidth<T>(this T source) where T : Control
        => source.CustomMinimumSize = new(0, source.CustomMinimumSize.Y);

    public static T LoseFocusOnEnter<T>(this T source) where T : LineEdit
    {
        source.GuiInput += OnInput;
        return source;

        void OnInput(InputEvent e)
        {
            if (e is InputEventKey key && key.PhysicalKeycode is Key.Enter or Key.KpEnter)
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
}
