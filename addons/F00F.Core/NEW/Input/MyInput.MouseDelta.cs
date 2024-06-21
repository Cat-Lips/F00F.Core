using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public partial class MyInput
{
    public Vector2 MouseDelta { get; private set; }
    public Vector2 MousePosition { get; private set; }

    #region Private

    private void SetMouseDelta(InputEvent e)
    {
        if (e is InputEventMouseMotion motion)
        {
            MouseDelta += motion.ScreenRelative;
            MousePosition = motion.GlobalPosition;
        }
    }

    private void ResetMouseDelta()
        => CallDeferred(MethodName._ResetMouseDelta);

    private void _ResetMouseDelta()
        => MouseDelta = Vector2.Zero;

    #endregion

    #region Xtras

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 GetMouseDelta() => Instance.MouseDelta;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 GetMousePosition() => Instance.MousePosition;

    #endregion
}
