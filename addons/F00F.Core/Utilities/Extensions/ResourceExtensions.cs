using Godot;

namespace F00F;

public static class ResourceExtensions
{
    public static string ResourceName(this Resource source)
        => string.IsNullOrEmpty(source.ResourceName) ? source.ResourcePath.GetFileName().Capitalise() : source.ResourceName;
}
