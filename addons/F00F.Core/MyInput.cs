using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;

namespace F00F
{
    public abstract class MyInput
    {
        private static readonly Settings<MyInput> SavedInput = new();

        protected static void Init<T>(string section = null, [CallerFilePath] string caller = null) where T : MyInput
        {
            var t = typeof(T);
            section ??= caller.GetFile().Split('.').First().TrimSuffix("Input");

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
                => actions.ForEach(x => x.SetValue(null, ActionName(x.Name)));

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
                            SavedInput.Get<InputEvent[]>(section, key).ForEach(e => InputMap.ActionAddEvent(action, e));
                        }
                    });
                }

                void ApplyRemainingDefaults()
                {
                    defaults.ForEach(x =>
                    {
                        var action = ActionName(x.Name);
                        if (!InputMap.HasAction(action))
                        {
                            InputMap.AddAction(action);

                            if (!x.FieldType.IsArray) AddEvent(x.GetValue(null));
                            else foreach (var e in (Array)x.GetValue(null)) AddEvent(e);
                        }

                        void AddEvent(object raw)
                        {
                            if (raw is InputEvent e) InputMap.ActionAddEvent(action, e);
                            else if (raw is Key key) InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = key });
                            else if (raw is JoyButton jb) InputMap.ActionAddEvent(action, new InputEventJoypadButton { ButtonIndex = jb });
                            else if (raw is MouseButton mb) InputMap.ActionAddEvent(action, new InputEventMouseButton { ButtonIndex = mb });
                        }
                    });
                }
            }
        }
    }
}
