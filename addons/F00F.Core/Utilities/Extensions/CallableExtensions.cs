using System;
using Godot;

namespace F00F
{
    public static class CallableExtensions
    {
        public static void CallDeferred(this GodotObject source, Action action)
        {
            Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(source))
                    action();
            }).CallDeferred();
        }

        public static void OnReady(this Node source, Action action)
        {
            source.Ready += action;
            if (source.IsInsideTree()) action();
        }
    }
}
