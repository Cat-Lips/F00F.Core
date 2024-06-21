using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public partial class MyInput
{
    public static event Action ActiveChanged;
    public static bool Active { get; private set { if (field != value) { field = value; ActiveChanged?.Invoke(); } } } = true;

    private static int count;
    public static void AddActiveItem(bool active)
        => Active = (count += active ? 1 : -1) is 0;

    #region Xtras

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetAxis(StringName negative, StringName positive)
        => Active ? Input.GetAxis(negative, positive) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetVector(StringName nX, StringName pX, StringName nY, StringName pY)
        => Active ? Input.GetVector(nX, pX, nY, pY) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsKeyPressed(Key code) => Active && Input.IsPhysicalKeyPressed(code);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionPressed(StringName action) => Active && Input.IsActionPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustPressed(StringName action) => Active && Input.IsActionJustPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustReleased(StringName action) => Active && Input.IsActionJustReleased(action);

    #region Obsolete?
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEvent e, float sensitivity, float pitchLimit) => Active && self.MouseRotate(e, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEventMouseMotion motion, float sensitivity, float pitchLimit) { if (!Active) return false; self.MouseRotate(motion, sensitivity, pitchLimit); return true; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEvent e, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) => Active && self.MouseOrbit(e, target, ref lookAt, pivotY, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEventMouseMotion motion, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) { if (!Active) return false; self.MouseOrbit(motion, target, ref lookAt, pivotY, sensitivity, pitchLimit); return true; }
    #endregion

    #endregion
}
