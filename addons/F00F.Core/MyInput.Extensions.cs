using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace F00F;

public partial class MyInput
{
    #region Mouse

    public static event Action ShowMouseSet;
    public static bool ShowMouse { get; set => OnShowMouseSet(ref field, value); }

    public static void ShowWithMouse(Control ui) => OnShowMouseSet(() => ui.Visible = ShowMouse);
    public static void HideWithMouse(Control ui) => OnShowMouseSet(() => ui.Visible = ShowMouse);
    public static void ShowWithMouse(CanvasLayer ui) => OnShowMouseSet(() => ui.Visible = !ShowMouse);
    public static void HideWithMouse(CanvasLayer ui) => OnShowMouseSet(() => ui.Visible = !ShowMouse);

    static MyInput() => ShowMouse = true;
    private static void OnShowMouseSet(ref bool field, bool show)
    {
        Input.MouseMode = show
            ? Input.MouseModeEnum.Visible
            : Input.MouseModeEnum.Captured;

        if (field != show)
        {
            field = show;
            ShowMouseSet?.Invoke();
        }
    }

    private static void OnShowMouseSet(Action action)
    {
        if (!Editor.IsEditor)
        {
            action();
            ShowMouseSet += action;
        }
    }

    #endregion

    #region Input

    public static float GetAxis(StringName negative, StringName positive)
        => Active ? Input.GetAxis(negative, positive) : default;

    public static Vector2 GetVector(StringName nX, StringName pX, StringName nY, StringName pY)
        => Active ? Input.GetVector(nX, pX, nY, pY) : default;

    public static bool IsKeyPressed(Key code) => Active && Input.IsPhysicalKeyPressed(code);
    public static bool IsActionPressed(StringName action) => Active && Input.IsActionPressed(action);
    public static bool IsActionJustPressed(StringName action) => Active && Input.IsActionJustPressed(action);

    #endregion

    #region Helpers

    private static readonly Dictionary<string, Dictionary<string, List<string>>> InputLookup = [];

    public static IEnumerable<string> Groups
        => InputLookup.Keys;

    public static IEnumerable<string> Actions(string group)
        => InputLookup[group].Keys;

    public static IEnumerable<string> Inputs(string group, string action)
        => InputLookup[group][action];

    private static void REGISTER_GROUP(string group)
        => InputLookup.Add(group, []);

    private static void REGISTER_ACTION(string group, string action)
        => InputLookup[group].Add(action, []);

    private static void REGISTER_INPUT(string group, string action, InputEvent e)
        => InputLookup[group][action].Add(e.AsText().TrimSuffix(" (Physical)"));

    public static string TabulateBB()
    {
        return string.Join('\n', TabulateBB());

        static IEnumerable<string> TabulateBB()
        {
            var first = true;
            yield return $"[table=2]";
            foreach (var (group, actions) in InputLookup)
            {
                if (!first)
                {
                    yield return CellSpace();
                    yield return CellSpace();
                }
                first = false;
                yield return Cell(Bold($"{group} Controls:"));
                yield return CellSpace();
                foreach (var (action, inputs) in actions)
                {
                    yield return Cell(action.Capitalise());
                    yield return Cell(string.Join(", ", inputs.Select(Bold)));
                }
            }
            yield return $"[/table]";

            static string Bold(string x)
                => $"[b]{x}[/b]";

            static string Cell(string x)
                => $"[cell]{x}[/cell]";

            static string CellSpace()
                => $"[cell]\n[/cell]";
        }
    }

    #endregion

    #region Activity

    public static event Action ActiveChanged;
    public static bool Active { get; set { if (field != value) { field = value; ActiveChanged?.Invoke(); } } } = true;

    private static int count;
    public static void AddActiveItem(bool active)
        => Active = (count += active ? 1 : -1) is 0;

    #endregion
}
