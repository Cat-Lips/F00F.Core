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
    }
}
