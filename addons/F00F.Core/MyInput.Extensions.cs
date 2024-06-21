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

    static MyInput() => ShowMouse = Input.MouseMode is Input.MouseModeEnum.Visible;
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

    public static void ShowWithMouse(params Control[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = ShowMouse));
    public static void HideWithMouse(params Control[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = !ShowMouse));
    public static void ShowWithMouse(params CanvasLayer[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = ShowMouse));
    public static void HideWithMouse(params CanvasLayer[] ui) => AddShowMouseAction(() => ui.ForEach(x => x.Visible = !ShowMouse));

    private static void AddShowMouseAction(Action action)
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
    public static bool IsActionJustReleased(StringName action) => Active && Input.IsActionJustReleased(action);

    public static bool MouseRotate(Node3D self, InputEvent e, float sensitivity, float pitchLimit) => Active && self.MouseRotate(e, sensitivity, pitchLimit);
    public static bool MouseRotate(Node3D self, InputEventMouseMotion motion, float sensitivity, float pitchLimit) { if (!Active) return false; self.MouseRotate(motion, sensitivity, pitchLimit); return true; }
    public static bool MouseOrbit(Node3D self, InputEvent e, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) => Active && self.MouseOrbit(e, target, ref lookAt, pivotY, sensitivity, pitchLimit);
    public static bool MouseOrbit(Node3D self, InputEventMouseMotion motion, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) { if (!Active) return false; self.MouseOrbit(motion, target, ref lookAt, pivotY, sensitivity, pitchLimit); return true; }

    #endregion

    #region Activity

    public static event Action ActiveChanged;
    public static bool Active { get; set { if (field != value) { field = value; ActiveChanged?.Invoke(); } } } = true;

    private static int count;
    public static void AddActiveItem(bool active)
        => Active = (count += active ? 1 : -1) is 0;

    #endregion

    #region Tabulate

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
}
