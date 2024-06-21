using Godot;

namespace F00F;

public static class Camera3DExtensions_Movement
{
    public static void PanCam(this Node3D self,
        in Vector2 mouse, float margin,
        in Vector2 screen, float speed,
        float delta)
    {
        var movement = Vector3.Zero;

        if (mouse.X < margin) movement.X = -1 * delta;
        else if (mouse.X > screen.X - margin) movement.X = 1 * delta;

        if (mouse.Y < margin) movement.Y = 1 * delta;
        else if (mouse.Y > screen.Y - margin) movement.Y = -1 * delta;

        if (movement != Vector3.Zero)
            self.TranslateObjectLocal(movement * speed);
    }
}
