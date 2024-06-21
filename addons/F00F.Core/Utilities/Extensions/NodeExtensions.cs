using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F
{
    public static class NodeExtensions
    {
        public static IEnumerable<T> GetChildren<T>(this Node source) where T : Node
            => source.GetChildren().OfType<T>();

        public static void ForEachChild<T>(this Node source, Action<T> action) where T : Node
            => source.GetChildren<T>().ForEach(action);

        public static void ForEachChild(this Node source, Action<Node> action)
            => source.GetChildren().ForEach(action);

        public static IEnumerable<T> RecurseChildren<T>(this Node source, bool self = false) where T : Node
            => source.SelectRecursive(x => x.GetChildren(), self).OfType<T>();

        public static IEnumerable<Node> RecurseChildren(this Node source, bool self = false)
            => source.SelectRecursive(x => x.GetChildren(), self);

        public static void RemoveChild(this Node source, Node node, bool free)
        {
            source.RemoveChild(node);
            if (free) node.QueueFree();
        }

        public static void RemoveChildren(this Node source, bool free)
            => source.GetChildren().ForEach(x => source.RemoveChild(x, free));

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

        public static Window GetMainWindowOrNull(this Node source)
            => GodotObject.IsInstanceValid(source) && source.IsInsideTree() ? source.GetTree().Root : null;

        public static T Get<[MustBeVariant] T>(this Node source, StringName property)
            => (T)(object)source.Get(property).AsGodotObject();
    }
}
