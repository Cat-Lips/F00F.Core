using System.Linq;
using Godot;

namespace F00F;

public static class ControlExtensions_OptionButton
{
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
