using Godot;

namespace F00F;

public static class PlaceholderExtensions
{
    public static T CreateInstance<T>(this InstancePlaceholder source) where T : Node
        => (T)source.CreateInstance();

    public static T CreateInstance<T>(this Node source) where T : Node
    {
        if (source is InstancePlaceholder ph)
            return ph.CreateInstance<T>();

        if (source is T t)
        {
            var copy = t.Copy();
            if (source.IsInTree())
                source.AddSibling(copy);
            return t;
        }

        return null;
    }
}
