using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F;

[Tool]
public abstract partial class MyInput
{
    private static readonly Config<MyInput> SavedInput = new();

    protected static void Init<T>(string section = null, [CallerFilePath] string caller = null) where T : MyInput
    {
        var t = typeof(T);
        if (Editor.IsEditor) return;
        section ??= caller.GetFile().Split('.').First().TrimSuffix("Input");
        REGISTER_GROUP(section);

        var actions = GetDeclaredActions(t);
        var defaults = GetDeclaredDefaults(t);

        SetActionNames();
        SetActionDefaults();

        StringName ActionName(string name)
            => new($"{section}.{name}");

        static FieldInfo[] GetDeclaredActions(Type t)
        {
            return t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(x => x.FieldType == typeof(StringName)).ToArray();
        }

        static FieldInfo[] GetDeclaredDefaults(Type t)
        {
            return t.GetNestedType("Defaults", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).ToArray();
        }

        void SetActionNames()
        {
            actions.ForEach(x =>
            {
                REGISTER_ACTION(section, x.Name);
                x.SetValue(null, ActionName(x.Name));
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
}
