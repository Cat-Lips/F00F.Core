using Godot;

namespace F00F
{
    public static class ReadabilityExtensions
    {
        public static void Enabled(this BaseButton source, bool enabled = true)
            => source.Disabled = !enabled;

        public static void Enabled(this CollisionShape3D source, bool enabled = true)
            => source.Disabled = !enabled;

        public static void ReplaceWith(this Node source, Node other, bool keepGroups = true)
            => source.ReplaceBy(other, keepGroups);

        public static T Copy<T>(this T source, bool subresources = true) where T : Resource
            => (T)source.Duplicate(subresources);

        public static T Copy<T>(this T source) where T : Node
            => (T)source.Duplicate();
    }
}
