using Godot;

namespace F00F;

public static class PlayerExtensions
{
    public static void ApplyJump(this IPlayer player, ref Vector3 velocity)
    {
        if (player.Grounded && player.Input.Jump())
            velocity.Y = player.Config.JumpStrength;
    }

    public static void ApplyGravity<T>(this T player, ref Vector3 velocity, float delta) where T : PhysicsBody3D, IPlayer
    {
        if (!player.Grounded)
            velocity += player.GetGravity() * delta;
    }

    public static void ApplyMovement<T>(this T player, ref Vector3 velocity) where T : Node3D, IPlayer
    {
        var input = player.Input.Movement();
        var dir = (player.Basis * input.V3()).Normalized();
        if (dir != Vector3.Zero)
        {
            velocity.X = dir.X * player.Config.MovementSpeed;
            velocity.Z = dir.Z * player.Config.MovementSpeed;
        }
        else
        {
            var current = player.Velocity;
            velocity.X = Mathf.MoveToward(current.X, 0, player.Config.MovementSpeed);
            velocity.Z = Mathf.MoveToward(current.Z, 0, player.Config.MovementSpeed);
        }
    }
}
