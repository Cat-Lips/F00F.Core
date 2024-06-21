using Godot;

namespace F00F;

public static class Sprite2DExtensions
{
    public static void Flip(this Sprite2D sprite, in Vector2 velocity)
    {
        if (velocity.X > 0) sprite.FlipH = false;
        else if (velocity.X < 0) sprite.FlipH = true;
    }

    public static void Flip(this Sprite2D sprite, bool flip = true)
        => sprite.FlipH = flip;

    public static bool IsFlipped(this Sprite2D sprite)
        => sprite.FlipH;
}
