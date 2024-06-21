using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

[Tool]
public abstract partial class InputConfig
{
    #region Init

    protected static void Init<T>(string section = null, [CallerFilePath] string caller = null) where T : InputConfig
    {
        var t = typeof(T);
        section ??= caller.GetFile().Split('.').First().TrimPrefix("My").TrimSuffix("Input");
        Init(t, section);
    }

    protected void Init(string section = null)
    {
        var t = GetType();
        section ??= t.Name.TrimPrefix("My").TrimSuffix("Input");
        Init(t, section, this);
    }

    #region Private

    private static readonly Config<MyInput> SavedInput = new();
    private static readonly Dictionary<string, Dictionary<string, List<string>>> InputLookup = [];

    private static void Init(Type t, string section, object instance = null)
    {
        REGISTER_GROUP(section);

        var actions = GetActions();
        var defaults = GetDefaults();

        SetActionNames();
        SetActionDefaults();

        StringName ActionName(string name)
            => new($"{section}.{name}");

        FieldInfo[] GetActions()
            => [.. t.GetFields().Where(x => x.FieldType == typeof(StringName))];

        FieldInfo[] GetDefaults()
            => [.. (t.GetNestedType("Default", BindingFlags.NonPublic)
                 ?? t.GetNestedType("Defaults", BindingFlags.NonPublic)).GetFields()];

        void SetActionNames()
        {
            actions.ForEach(x =>
            {
                REGISTER_ACTION(section, x.Name);
                x.SetValue(x.IsStatic ? null : instance, ActionName(x.Name));
            });
        }

        void SetActionDefaults()
        {
            LoadCurrentSettings();
            ApplyRemainingDefaults();

            void LoadCurrentSettings()
            {
                SavedInput.Keys(section).ForEach(key =>
                {
                    var action = ActionName(key);
                    if (!InputMap.HasAction(action))
                    {
                        InputMap.AddAction(action);

                        foreach (var e in SavedInput.Get<InputEvent[]>(section, key))
                        {
                            REGISTER_INPUT(section, key, e);
                            InputMap.ActionAddEvent(action, e);
                        }
                    }
                });
            }

            void ApplyRemainingDefaults()
            {
                defaults.ForEach(x =>
                {
                    var key = x.Name;
                    var action = ActionName(key);
                    if (!InputMap.HasAction(action))
                    {
                        InputMap.AddAction(action);

                        if (!x.FieldType.IsArray) AddEvent(x.GetValue(null));
                        else foreach (var e in (Array)x.GetValue(null)) AddEvent(e);
                    }

                    void AddEvent(object raw)
                    {
                        var e = InputEvent();

                        REGISTER_INPUT(section, key, e);
                        InputMap.ActionAddEvent(action, e);

                        InputEvent InputEvent()
                        {
                            return raw is Key key ? new InputEventKey { PhysicalKeycode = key }
                                 : raw is JoyButton jb ? new InputEventJoypadButton { ButtonIndex = jb }
                                 : raw is MouseButton mb ? new InputEventMouseButton { ButtonIndex = mb }
                                 : (InputEvent)raw;
                        }
                    }
                });
            }
        }
    }

    #region Utils

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void REGISTER_GROUP(string group)
        => InputLookup.Add(group, []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void REGISTER_ACTION(string group, string action)
        => InputLookup[group].Add(action, []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void REGISTER_INPUT(string group, string action, InputEvent e)
        => InputLookup[group][action].Add(e.AsText().TrimSuffix(" (Physical)"));

    #endregion

    #endregion

    #endregion

    #region Print

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

    #region Xtras (tmp)

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 GetMouseDelta() => MyInput.GetMouseDelta();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 GetMousePosition() => MyInput.GetMousePosition();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void ShowWithMouse(params Control[] ui) => MyInput.ShowWithMouse(ui);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HideWithMouse(params Control[] ui) => MyInput.HideWithMouse(ui);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void ShowWithMouse(params CanvasLayer[] ui) => MyInput.ShowWithMouse(ui);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HideWithMouse(params CanvasLayer[] ui) => MyInput.HideWithMouse(ui);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool GetMouseVisible() => MyInput.GetMouseVisible();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SetMouseVisible(bool visible) => MyInput.SetMouseVisible(visible);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void OnMouseVisibilityChanged(Action action) => MyInput.OnMouseVisibilityChanged(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float GetAxis(StringName negative, StringName positive) => MyInput.GetAxis(negative, positive);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 GetVector(StringName nX, StringName pX, StringName nY, StringName pY) => MyInput.GetVector(nX, pX, nY, pY);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsKeyPressed(Key code) => MyInput.IsKeyPressed(code);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionPressed(StringName action) => MyInput.IsActionPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustPressed(StringName action) => MyInput.IsActionJustPressed(action);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsActionJustReleased(StringName action) => MyInput.IsActionJustReleased(action);

    #region Obsolete?
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEvent e, float sensitivity, float pitchLimit) => MyInput.MouseRotate(self, e, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseRotate(Node3D self, InputEventMouseMotion motion, float sensitivity, float pitchLimit) => MyInput.MouseRotate(self, motion, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEvent e, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) => MyInput.MouseOrbit(self, e, target, ref lookAt, pivotY, sensitivity, pitchLimit);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool MouseOrbit(Node3D self, InputEventMouseMotion motion, in Transform3D target, ref Vector3 lookAt, bool pivotY, float sensitivity, float pitchLimit) => MyInput.MouseOrbit(self, motion, target, ref lookAt, pivotY, sensitivity, pitchLimit);
    #endregion

    #endregion
}
