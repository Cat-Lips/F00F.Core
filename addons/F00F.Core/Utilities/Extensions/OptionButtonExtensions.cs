using System;
using System.Collections.Generic;
using Godot;

namespace F00F;

using IconItem = (string Key, Texture2D Icon);

public static class OptionButtonExtensions
{
    #region Key Items

    public static void SetItems(this OptionButton self, IEnumerable<string> keys)
    {
        self.Clear();
        self.AddItems(keys);
    }

    public static void AddItems(this OptionButton self, IEnumerable<string> keys)
    {
        keys?.ForEach(x =>
        {
            var idx = self.ItemCount;
            self.AddItem(x.Capitalise());
            self.SetItemMetadata(idx, x);
        });
    }

    public static string GetItem(this OptionButton self, int idx)
        => (string)self.GetItemMetadata(idx);

    public static string GetSelectedItem(this OptionButton self)
        => (string)self.GetSelectedMetadata();

    public static void SetSelectedItem(this OptionButton self, string key)
    {
        if (key != self.GetSelectedItem())
            self.Select(self.IndexOf(key));
    }

    public static int IndexOf(this OptionButton self, string key)
    {
        var count = self.ItemCount;
        for (var idx = 0; idx < count; ++idx)
        {
            if (key == self.GetItem(idx))
                return idx;
        }

        return -1;
    }

    #endregion

    #region Enum Items

    public static void SetItems<T>(this OptionButton self) where T : struct, Enum
    {
        self.Clear();
        self.AddItems<T>();
    }

    public static void AddItems<T>(this OptionButton self) where T : struct, Enum
    {
        Enum.GetValues<T>().ForEach(x =>
        {
            var idx = self.ItemCount;
            var id = Convert.ToInt32(x);
            self.AddItem(x.Capitalise(), id);
        });
    }

    public static T GetItem<T>(this OptionButton self, int idx) where T : struct, Enum
        => (T)Enum.ToObject(typeof(T), self.GetItemId(idx));

    public static T GetSelectedItem<T>(this OptionButton self) where T : struct, Enum
        => (T)Enum.ToObject(typeof(T), self.GetSelectedId());

    public static void SetSelectedItem<T>(this OptionButton self, T value) where T : struct, Enum
    {
        var id = Convert.ToInt32(value);
        if (id != self.GetSelectedId())
        {
            var count = self.ItemCount;
            for (var idx = 0; idx < count; ++idx)
            {
                if (id == self.GetItemId(idx))
                {
                    self.Select(idx);
                    break;
                }
            }
        }
    }

    #endregion

    #region Icon Items

    public static void SetItems(this OptionButton self, IEnumerable<IconItem> icons)
    {
        self.Clear();
        self.AddItems(icons);
    }

    public static void AddItems(this OptionButton self, IEnumerable<IconItem> icons)
        => icons.ForEach(x => self.AddItem(x.Key, x.Icon));

    public static void AddItem(this OptionButton self, string key, Texture2D icon)
    {
        var idx = self.ItemCount;
        var txt = key.Capitalise();
        self.AddIconItem(icon, null);
        self.SetItemTooltip(idx, txt);
        self.SetItemMetadata(idx, key);
    }

    #endregion

    // *** Unused? *** //

    //public static void AddItems<T>(this OptionButton self, T selected = default, Func<T, string> mod = null) where T : struct, Enum
    //{
    //    mod ??= t => t.Str();
    //    foreach (var value in Enum.GetValues<T>())
    //    {
    //        var displayName = mod(value);
    //        var id = Convert.ToInt32(value);
    //        self.AddItem(displayName, id);

    //        if (EqualityComparer<T>.Default.Equals(value, selected))
    //            self.Selected = self.ItemCount - 1;
    //    }
    //}

    //public static void SetSelected<T>(this OptionButton self, T? selected = null) where T : struct, Enum
    //    => self.Selected = selected.Index();

    //public static T GetSelected<T>(this OptionButton self) where T : struct, Enum
    //    => (T)Enum.ToObject(typeof(T), self.GetSelectedId());

    //public static void AddItems(this OptionButton source, params string[] items)
    //    => items.ForEach(x => source.AddItem(x));

    //public static void AddItem(this OptionButton source, string key)
    //{
    //    var idx = source.ItemCount;
    //    var text = key.Capitalise();
    //    source.AddItem(text);
    //    source.SetItemMetadata(idx, key);
    //}

    //public static void AddItems(this OptionButton source, params (string Key, Texture2D Icon)[] items)
    //    => items.ForEach(x => source.AddItem(x.Key, x.Icon));

    //public static void AddItem(this OptionButton source, string key, Texture2D icon)
    //{
    //    var idx = source.ItemCount;
    //    var text = key.Capitalise();
    //    source.AddIconItem(icon, null);
    //    source.SetItemTooltip(idx, text);
    //    source.SetItemMetadata(idx, key);
    //}

    //public static string GetItemKey(this OptionButton source, int idx)
    //    => (string)source.GetItemMetadata(idx);

    //public static string GetSelectedKey(this OptionButton source)
    //    => (string)source.GetSelectedMetadata();

    //public static int IndexOf(this OptionButton source, string key)
    //{
    //    return Enumerable.Range(0, source.ItemCount)
    //        .FirstOrDefault(idx => source.GetItemKey(idx) == key, -1);
    //}

    #region Readability & Convenience

    public static void EnableItem(this OptionButton source, int idx)
        => source.SetItemDisabled(idx, false);

    public static void DisableItem(this OptionButton source, int idx)
    {
        source.SetItemDisabled(idx, true);
        if (idx == source.Selected)
            source.SelectNext();
    }

    public static bool IsItemEnabled(this OptionButton source, int idx)
        => !source.IsItemDisabled(idx);

    //public static bool IsItemDisabled(this OptionButton source, int idx)
    //    => source.IsItemDisabled(idx);

    public static IEnumerable<int> GetEnabledItems(this OptionButton source)
    {
        var count = source.ItemCount;
        for (var idx = 0; idx < count; ++idx)
        {
            if (!source.IsItemDisabled(idx) && !source.IsItemSeparator(idx))
                yield return idx;
        }
    }

    public static IEnumerable<int> GetDisabledItems(this OptionButton source)
    {
        var count = source.ItemCount;
        for (var idx = 0; idx < count; ++idx)
        {
            if (source.IsItemDisabled(idx) && !source.IsItemSeparator(idx))
                yield return idx;
        }
    }

    public static void SelectFirst(this OptionButton source, int from = 0)
    {
        var idx = FirstAvailableItem(from);
        source.Selected = idx;
        source.EmitSignal(OptionButton.SignalName.ItemSelected, idx);

        int FirstAvailableItem(int from)
        {
            var count = source.ItemCount;
            for (var idx = from; idx < count; ++idx)
            {
                if (!source.IsItemDisabled(idx) && !source.IsItemSeparator(idx))
                    return idx;
            }

            return from is 0 ? -1 : source.GetSelectableItem();
        }
    }

    public static void SelectNext(this OptionButton source, int? from = null)
        => source.SelectFirst((from ?? source.Selected) + 1);

    #endregion
}
