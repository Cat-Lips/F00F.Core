using Godot;

namespace F00F;

public static class PlayerConfigExtensions
{
    public static void Init<T>(this PlayerConfig cfg, T root) where T : Node3D, IPlayer
    {
        // TODO
    }
}
