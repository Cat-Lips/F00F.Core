using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class NodeExtensions
{
    public static IEnumerable<T> GetChildren<T>(this Node source)
        => source.GetChildren().OfType<T>();

    public static void ForEachChild<T>(this Node source, Action<T> action)
        => source.GetChildren<T>().ForEach(action);

    public static void ForEachChild(this Node source, Action<Node> action)
        => source.GetChildren().ForEach(action);

    public static IEnumerable<T> RecurseChildren<T>(this Node source, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self).OfType<T>();

    public static IEnumerable<Node> RecurseChildren(this Node source, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self);

    public static void AddChild(this Node source, Node child, bool own)
    {
        child.Owner = null;
        source.AddChild(child, forceReadableName: true);
        if (own) child.Owner = source.Owner ?? source;
    }

    public static void AddSibling(this Node source, Node sibling, bool own)
    {
        sibling.Owner = null;
        source.AddSibling(sibling, forceReadableName: true);
        if (own) sibling.Owner = source.Owner ?? source;
    }

    public static void Reparent(this Node source, Node newParent, bool own)
    {
        source.Owner = null;
        source.Reparent(newParent);
        if (own) source.Owner = newParent.Owner ?? newParent;
    }

    public static void AddChildren(this Node source, IEnumerable<Node> child, bool own)
        => child.ForEach(x => source.AddChild(x, own: own));

    public static void AttachChild(this Node source, Node parent, bool own)
        => parent.AddChild(source, own: own);

    public static void DetachChild(this Node source, bool free)
        => source.GetParent().RemoveChild(source, free: free);

    public static void RemoveChild(this Node source, Node node, bool free)
    {
        source.RemoveChild(node);
        if (free) node.QueueFree();
    }

    public static void RemoveChildren(this Node source)
        => source.GetChildren().ForEach(x => source.RemoveChild(x, free: true));

    public static void RemoveChildren<T>(this Node source)
        => source.GetChildren<T>().ForEach(x => source.RemoveChild(x as Node, free: true));

    public static void RemoveChildren(this Node source, Func<Node, bool> predicate)
    {
        source.RecurseChildren().Where(predicate)
            .ForEach(x => x.DetachChild(free: true));
    }

    public static bool HasParent(this Node source)
        => source.GetParent() is not null;

    public static bool HasParent<T>(this Node source) where T : class
        => source.GetParentOrNull<T>() is not null;

    public static bool HasParent<T>(this Node source, out T parent) where T : class
        => (parent = source.GetParentOrNull<T>()) is not null;

    public static T FindParent<T>(this Node source) where T : class
    {
        var parent = source.GetParent();
        return parent as T ?? parent?.FindParent<T>();
    }

    public static Aabb GetMeshAabb(this Node source, bool self = false)
    {
        return source.RecurseChildren<MeshInstance3D>(self).IfAny(_ => _
            .Select(x => x.GetAabb().TransformedBy(x.GlobalTransform))
            .Aggregate((a, b) => a.Merge(b)));
    }
}
