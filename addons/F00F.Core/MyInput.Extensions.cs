using System;
using Godot;

namespace F00F;

public partial class MyInput
{
    #region Input

    public static float GetAxis(StringName negative, StringName positive)
        => Active ? Input.GetAxis(negative, positive) : default;

    public static Vector2 GetVector(StringName nX, StringName pX, StringName nY, StringName pY)
        => Active ? Input.GetVector(nX, pX, nY, pY) : default;

    public static bool IsKeyLabelPressed(Key code) => Active && Input.IsKeyLabelPressed(code);

    public static bool IsActionPressed(StringName action) => Active && Input.IsActionPressed(action);
    public static bool IsActionJustPressed(StringName action) => Active && Input.IsActionJustPressed(action);

    #endregion

    #region Activity

    public static event Action ActiveChanged;
    public static bool Active { get; set { if (field != value) { field = value; ActiveChanged?.Invoke(); } } } = true;

    private static int count;
    public static void AddActiveItem(bool active)
        => Active = (count += active ? 1 : -1) is 0;

    #endregion
}
