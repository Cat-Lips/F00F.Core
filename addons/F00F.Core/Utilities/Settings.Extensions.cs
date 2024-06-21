using System;
using Godot;

namespace F00F
{
    public static class SettingsExtensions
    {
        #region Enums

        public static void SetEnum<T>(this Settings source, string key, T value) where T : struct, Enum => source.Set(key, $"{value}");
        public static void SetEnum<T>(this Settings source, string section, string key, T value) where T : struct, Enum => source.Set(section, key, $"{value}");

        public static T? GetEnum<T>(this Settings source, string key) where T : struct, Enum => source.TryGetEnum<T>(key, out var value) ? value : null;
        public static T? GetEnum<T>(this Settings source, string section, string key) where T : struct, Enum => source.TryGetEnum<T>(section, key, out var value) ? value : null;

        public static T GetEnum<T>(this Settings source, string key, T @default) where T : struct, Enum => source.TryGetEnum<T>(key, out var value) ? value : @default;
        public static T GetEnum<T>(this Settings source, string section, string key, T @default) where T : struct, Enum => source.TryGetEnum<T>(section, key, out var value) ? value : @default;

        public static bool TryGetEnum<T>(this Settings source, string key, out T value) where T : struct, Enum => source.TryGetEnum("", key, out value);
        public static bool TryGetEnum<T>(this Settings source, string section, string key, out T value) where T : struct, Enum
        {
            if (source.TryGet<string>(section, key, out var str))
            {
                value = Enum.Parse<T>(str);
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region Nodes

        public static void Set<[MustBeVariant] T>(this Settings source, Node key, T value) => source.Set(key.Name, value);
        public static void Set<[MustBeVariant] T>(this Settings source, Node section, Node key, T value) => source.Set(section.Name, key.Name, value);

        public static T Get<[MustBeVariant] T>(this Settings source, Node key) => source.Get<T>(key.Name);
        public static T Get<[MustBeVariant] T>(this Settings source, Node section, Node key) => source.Get<T>(section.Name, key.Name);

        public static T Get<[MustBeVariant] T>(this Settings source, Node key, T @default) => source.Get(key.Name, @default);
        public static T Get<[MustBeVariant] T>(this Settings source, Node section, Node key, T @default) => source.Get(section.Name, key.Name, @default);

        public static bool TryGet<[MustBeVariant] T>(this Settings source, Node key, out T value) => source.TryGet(key.Name, out value);
        public static bool TryGet<[MustBeVariant] T>(this Settings source, Node section, Node key, out T value) => source.TryGet(section.Name, key.Name, out value);

        public static void Clear(this Settings source, Node key) => source.Clear(key.Name);
        public static void Clear(this Settings source, Node section, Node key) => source.Clear(section.Name, key.Name);

        #endregion
    }
}
