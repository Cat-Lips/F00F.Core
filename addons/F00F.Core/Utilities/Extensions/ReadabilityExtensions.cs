using System.Threading;
using Godot;

namespace F00F;

public static class ReadabilityExtensions
{
    public static void Enabled(this BaseButton source, bool enabled = true)
        => source.Disabled = !enabled;

    public static void Enabled(this CollisionShape3D source, bool enabled = true)
        => source.Disabled = !enabled;

    public static void Enabled(this SpinBox source, bool enabled = true)
        => source.Editable = enabled;

    public static void ReplaceWith(this Node source, Node other, bool keepGroups = true, bool free = true)
    {
        source.ReplaceBy(other, keepGroups);
        if (free) source.QueueFree();
    }

    public static T Copy<T>(this T source, bool deep = true) where T : Resource
        => (T)source.Duplicate(deep);

    public static T Copy<T>(this T source) where T : Node
        => (T)source.Duplicate();

    public static bool Cancelled(this in CancellationToken ct)
        => ct.IsCancellationRequested;

    public static bool IsReady(this Node source)
        => source.IsNodeReady();

    public static bool NotReady(this Node source)
        => !source.IsNodeReady();

    public static Node GetNode(this NodePath source, Node root)
        => source?.IsEmpty is null or true ? null : root.GetNodeOrNull(source);

    public static T GetNode<T>(this NodePath source, Node root) where T : Node
        => source?.IsEmpty is null or true ? null : (T)root.GetNodeOrNull(source);
}
