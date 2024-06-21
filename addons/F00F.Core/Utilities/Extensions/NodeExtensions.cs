using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public static class NodeExtensions
{
    public static IEnumerable<T> GetChildren<T>(this Node source)
        => source.GetChildren().OfType<T>();

    //public static IEnumerable<T> GetChildren<T>(this Node source, Func<T, bool> where)
    //    => source.GetChildren().OfType<T>().Where(where);

    public static void ForEachChild<T>(this Node source, Action<T> action)
        => source.GetChildren<T>().ForEach(action);

    public static void ForEachChild<T>(this Node source, Func<T, bool> where, Action<T> action)
        => source.GetChildren<T>().Where(where).ForEach(action);

    public static void ForEachChild(this Node source, Action<Node> action)
        => source.GetChildren().ForEach(action);

    public static void ForEachChild(this Node source, Func<Node, bool> where, Action<Node> action)
        => source.GetChildren().Where(where).ForEach(action);

    public static IEnumerable<T> RecurseChildren<T>(this Node source, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self).OfType<T>();

    public static IEnumerable<T> RecurseChildren<T>(this Node source, Func<T, bool> where, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self).OfType<T>().Where(where);

    public static IEnumerable<Node> RecurseChildren(this Node source, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self);

    public static IEnumerable<Node> RecurseChildren(this Node source, Func<Node, bool> where, bool self = false)
        => source.SelectRecursive(x => x.GetChildren(), self).Where(where);

    public static void AddChild(this Node source, Node child, Node owner)
    {
        child.Owner = null;
        source.AddChild(child, forceReadableName: true);
        child.Owner = owner;
    }

    public static void AddSibling(this Node source, Node sibling, Node owner)
    {
        sibling.Owner = null;
        source.AddSibling(sibling, forceReadableName: true);
        sibling.Owner = owner;
    }

    public static void Reparent(this Node source, Node newParent, Node owner)
    {
        source.Owner = null;
        source.Reparent(newParent);
        source.Owner = owner;
    }

    public static void AddChildren(this Node source, IEnumerable<Node> child, Node owner)
        => child.ForEach(x => source.AddChild(x, owner));

    public static void AddChildren(this Node source, IEnumerable<Node> child)
        => child.ForEach(x => source.AddChild(x));

    public static void AttachChild(this Node source, Node parent, Node owner)
        => parent.AddChild(source, owner);

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

    public static void RemoveChildren(this Node source, Func<Node, bool> where)
        => source.GetChildren().Where(where).ForEach(x => x.DetachChild(free: true));

    public static void RemoveChildren<T>(this Node source, Func<T, bool> where)
        => source.GetChildren<T>().Where(where).ForEach(x => (x as Node).DetachChild(free: true));

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

    public static Node GetSibling(this Node source, NodePath path)
        => source.GetParent().GetNode(path);

    public static T GetSibling<T>(this Node source, NodePath path) where T : Node
        => (T)source.GetSibling(path);

    public static Node GetSiblingOrNull(this Node source, NodePath path)
        => source.GetParent()?.GetNodeOrNull(path);

    public static T GetSiblingOrNull<T>(this Node source, NodePath path) where T : Node
        => (T)source.GetSiblingOrNull(path);

    public static Aabb BB(this Node source) => source.GetMeshAabb();
    public static Aabb Bounds(this Node source) => source.GetMeshAabb();
    public static Aabb GetBounds(this Node source) => source.GetMeshAabb();
    public static Aabb GetMeshAabb(this Node source)
    {
        return source.RecurseChildren<MeshInstance3D>(self: true).IfAny(_ => _
            .Select(x => x.GetAabb().TransformedBy(x.GlobalTransform))
            .Aggregate((a, b) => a.Merge(b)));
    }

    public static T ValidOrNull<T>(this T source) where T : Node
        => GodotObject.IsInstanceValid(source) ? source : null;
}
