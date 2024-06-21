using System;
using System.Linq;
using Godot;

namespace F00F;

public static class InputExtensions
{
    public static bool MouseRotate(this Node3D source, InputEvent e, float sensitivity, float pitchLimit)
    {
        if (e is not InputEventMouseMotion motion) return false;

        var (x, y) = -motion.Relative * sensitivity;

        var rotation = source.Rotation;

        rotation.X = ClampPitch(rotation.X += y);
        rotation.Y += x;
        rotation.Z = 0;

        source.Rotation = rotation;
        return true;

        float ClampPitch(float x)
            => Mathf.Clamp(x, -pitchLimit, pitchLimit);
    }

    public static bool MouseRotateAround(this Node3D source, InputEvent e, in Vector3 target, float sensitivity, float pitchLimit)
    {
        if (e is not InputEventMouseMotion motion) return false;

        var (x, y) = -motion.Relative * sensitivity;

        var xform = source.Transform;

        var hRot = new Quaternion(xform.Basis.Y, x);
        var vRot = new Quaternion(xform.Basis.X, y);
        var rotation = hRot * vRot;

        var curDirection = xform.Origin - target;
        var newDirection = rotation * curDirection;

        source.Transform = new Transform3D(new Basis(rotation), target + newDirection);
        source.LookAt(target);
        return true;
    }

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

    public static void Handled(this Node source)
        => source.GetViewport().SetInputAsHandled();
}
