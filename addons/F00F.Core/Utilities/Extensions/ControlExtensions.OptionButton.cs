using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class ControlExtensions_OptionButton
{
    public static void AddItems<T>(this OptionButton self, T selected = default, Func<T, string> mod = null) where T : struct, Enum
    {
        mod ??= t => t.Str();
        foreach (var value in Enum.GetValues<T>())
        {
            var displayName = mod(value);
            var id = Convert.ToInt32(value);
            self.AddItem(displayName, id);

            if (EqualityComparer<T>.Default.Equals(value, selected))
                self.Selected = self.ItemCount - 1;
        }
    }

    public static void SetSelected<T>(this OptionButton self, T? selected = null) where T : struct, Enum
        => self.Selected = selected.Index();

    public static T GetSelected<T>(this OptionButton self) where T : struct, Enum
        => (T)Enum.ToObject(typeof(T), self.GetSelectedId());

    public static void AddItems(this OptionButton source, params string[] items)
        => items.ForEach(x => source.AddItem(x));

    public static void AddItem(this OptionButton source, string key)
    {
        var idx = source.ItemCount;
        var text = key.Capitalise();
        source.AddItem(text);
        source.SetItemMetadata(idx, key);
    }

    public static void AddItems(this OptionButton source, params (string Key, Texture2D Icon)[] items)
        => items.ForEach(x => source.AddItem(x.Key, x.Icon));

    public static void AddItem(this OptionButton source, string key, Texture2D icon)
    {
        var idx = source.ItemCount;
        var text = key.Capitalise();
        source.AddIconItem(icon, null);
        source.SetItemTooltip(idx, text);
        source.SetItemMetadata(idx, key);
    }

    public static string GetItemKey(this OptionButton source, int idx)
        => (string)source.GetItemMetadata(idx);

    public static string GetSelectedKey(this OptionButton source)
        => (string)source.GetSelectedMetadata();

    public static int IndexOf(this OptionButton source, string key)
    {
        return Enumerable.Range(0, source.ItemCount)
            .FirstOrDefault(idx => source.GetItemKey(idx) == key, -1);
    }
}
