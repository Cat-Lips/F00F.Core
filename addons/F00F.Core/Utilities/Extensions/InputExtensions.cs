using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public static class InputExtensions
{
    #region MouseRotate

    public static void MouseRotate(this Node3D source, InputEventMouseMotion motion, float sensitivity, float pitchLimit)
    {
        var (x, y) = -motion.ScreenRelative * sensitivity;

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
        var (x, y) = -motion.ScreenRelative * sensitivity;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Handled(this Node source) => source.GetViewport().SetInputAsHandled();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Pressed(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && e.Pressed;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Released(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && !e.Pressed;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool JustPressed(this InputEventKey e, Key key) => MyInput.Active && e.PhysicalKeycode == key && e.Pressed && !e.Echo;

    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsKey(this InputEvent e) => e is InputEventKey;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsKey(this InputEvent e, out InputEventKey key) => (key = e as InputEventKey).NotNull();

    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMouseButton(this InputEvent e) => e is InputEventMouseButton;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMouseButton(this InputEvent e, out InputEventMouseButton mb) => (mb = e as InputEventMouseButton).NotNull();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMouseMotion(this InputEvent e) => e is InputEventMouseMotion;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsMouseMotion(this InputEvent e, out InputEventMouseMotion mm) => (mm = e as InputEventMouseMotion).NotNull();

    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InputEventKey AsKey(this InputEvent e) => e as InputEventKey;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InputEventMouseButton AsMouseButton(this InputEvent e) => e as InputEventMouseButton;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static InputEventMouseMotion AsMouseMotion(this InputEvent e) => e as InputEventMouseMotion;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionPressed(this InputEvent e, StringName action) => MyInput.Active && e.IsActionPressed(action, allowEcho: true);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustPressed(this InputEvent e, StringName action) => MyInput.Active && e.IsActionPressed(action, allowEcho: false);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustReleased(this InputEvent e, StringName action) => MyInput.Active && e.IsActionReleased(action);
}
