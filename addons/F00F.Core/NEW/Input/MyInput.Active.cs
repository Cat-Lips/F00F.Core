using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public partial class MyInput
{
    public event Action ActiveChanged;
    public bool Active { get; private set => this.Set(ref field, value, ActiveChanged); } = true;

    private int count;
    public void AddActiveItem(bool active)
        => Active = (count += active ? 1 : -1) is 0;

    #region Xtras

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetAxis(StringName negative, StringName positive)
        => Instance.Active ? Input.GetAxis(negative, positive) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetVector(StringName nX, StringName pX, StringName nY, StringName pY)
        => Instance.Active ? Input.GetVector(nX, pX, nY, pY) : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsKeyPressed(Key code) => Instance.Active && Input.IsPhysicalKeyPressed(code);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionPressed(StringName action) => Instance.Active && Input.IsActionPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustPressed(StringName action) => Instance.Active && Input.IsActionJustPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustReleased(StringName action) => Instance.Active && Input.IsActionJustReleased(action);

    #region Obsolete?
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEvent e, float sensitivity, float pitchLimit) => Instance.Active && self.MouseRotate(e, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEventMouseMotion motion, float sensitivity, float pitchLimit) { if (!Instance.Active) return false; self.MouseRotate(motion, sensitivity, pitchLimit); return true; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEvent e, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) => Instance.Active && self.MouseOrbit(e, target, ref lookAt, pivotY, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEventMouseMotion motion, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) { if (!Instance.Active) return false; self.MouseOrbit(motion, target, ref lookAt, pivotY, sensitivity, pitchLimit); return true; }
    #endregion

    #endregion
}
