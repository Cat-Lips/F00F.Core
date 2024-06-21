using System;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

public partial class MyInput : Node
{
    public event Action MouseVisibilityChanged;

    public bool MouseVisible { get; set => this.Set(ref field, value, SetMouseMode, MouseVisibilityChanged); } = true;

    #region Private

    private void SetMouseMode()
    {
        Input.MouseMode = MouseVisible
            ? Input.MouseModeEnum.Visible
            : Input.MouseModeEnum.Captured;
    }

    private void SetMouseVisibility()
        => MouseVisible = Input.MouseMode == Input.MouseModeEnum.Visible;

    #endregion

    #region Xtras

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void ShowWithMouse(params Control[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = Instance.MouseVisible));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HideWithMouse(params Control[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = !Instance.MouseVisible));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void ShowWithMouse(params CanvasLayer[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = Instance.MouseVisible));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HideWithMouse(params CanvasLayer[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = !Instance.MouseVisible));

    #region Private

    private static void AddShowMouseAction(Action action)
    {
        if (!Editor.IsEditor)
        {
            action();
            Instance.MouseVisibilityChanged += action;
        }
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool GetMouseVisible() => Instance.MouseVisible;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SetMouseVisible(bool visible = true) => Instance.MouseVisible = visible;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void OnMouseVisibilityChanged(Action action) => Instance.MouseVisibilityChanged += action;

    #endregion
}
