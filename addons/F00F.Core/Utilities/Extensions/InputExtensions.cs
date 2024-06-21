using System;
using System.Linq;
using Godot;

namespace F00F;

public static class InputExtensions
{
    #region MouseRotate

    public static void MouseRotate(this Node3D source, InputEventMouseMotion motion, float sensitivity, float pitchLimit)
    {
        var (x, y) = -motion.Relative * sensitivity;

        var rotation = source.Rotation;

        rotation.X = ClampPitch(rotation.X += y);
        rotation.Y += x;
        rotation.Z = 0;

        source.Rotation = rotation;

        float ClampPitch(float x)
            => Mathf.Clamp(x, -pitchLimit, pitchLimit);
    }

    public static bool MouseRotate(this Node3D source, InputEvent e, float sensitivity, float pitchLimit)
    {
        if (e is InputEventMouseMotion motion)
        {
            source.MouseRotate(motion, sensitivity, pitchLimit);
            return true;
        }

        return false;
    }

    #endregion

    #region MouseOrbit

    public static void MouseOrbit(this Node3D source, InputEventMouseMotion motion, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit)
    {
        var (x, y) = -motion.Relative * sensitivity;

        var xform = source.Transform;

        var curDirection = xform.Origin - target.Origin;
        var curDistance = curDirection.Length();

        var yaw = Mathf.Atan2(curDirection.X, curDirection.Z) + x;
        var pitch = ClampPitch(Mathf.Asin(curDirection.Y / curDistance) + y);
        var hDist = curDistance * Mathf.Cos(pitch);

        var newDirection = new Vector3(
            hDist * Mathf.Sin(yaw),
            curDistance * Mathf.Sin(pitch),
            hDist * Mathf.Cos(yaw));

        lookAt = lookAt.Rotated(target.Up(), x);
        if (pivotY) lookAt = lookAt.Rotated(target.Right(), y);

        var newPosition = target.Origin + newDirection;
        source.LookAtFromPosition(newPosition, target.Origin + lookAt);

        float ClampPitch(float x)
            => Mathf.Clamp(x, -pitchLimit, pitchLimit);
    }

    public static bool MouseOrbit(this Node3D source, InputEvent e, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit)
    {
        if (e is InputEventMouseMotion motion)
        {
            source.MouseOrbit(motion, target, ref lookAt, pivotY, sensitivity, pitchLimit);
            return true;
        }

        return false;
    }

    #endregion

    #region Handle

    public static bool Handle(this Node source, bool handle)
    {
        if (handle)
        {
            source.Handled();
            return true;
        }

        return false;
    }

    public static bool Handle(this Node source, bool handle, params Action[] actions)
    {
        if (handle)
        {
            actions.ForEach(x => x?.Invoke());
            source.Handled();
            return true;
        }

        return false;
    }

    public static bool Handle(this Node source, bool handle, params Func<bool>[] actions)
    {
        if (handle)
        {
            if (actions.Any(x => x?.Invoke() ?? false))
            {
                source.Handled();
                return true;
            }
        }

        return false;
    }

    #endregion

    public static void Handled(this Node source)
        => source.GetViewport().SetInputAsHandled();

    public static bool Pressed(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && e.Pressed;
    public static bool Released(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && !e.Pressed;
    public static bool JustPressed(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && e.Pressed && !e.Echo;
}
