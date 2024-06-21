using System;
using Godot;

namespace F00F;

public static class ConfigExtensions
{
    public static ConfigFile LoadConfig(this string path, bool autoSave = true)
    {
        var cfg = new ConfigFile();

        if (autoSave)
            cfg.SetMeta("File", path);

        if (FS.FileExists(path))
            cfg.Load(path);

        return cfg;
    }

    public static T Get<[MustBeVariant] T>(this ConfigFile cfg, string key, T @default = default) => cfg.Get("", key, @default);
    public static T Get<[MustBeVariant] T>(this ConfigFile cfg, string section, string key, T @default = default)
    {
        if (cfg.HasSectionKey(section, key))
            return cfg.GetValue<T>(section, key);

        cfg.SetValue(section, key, @default);

        if (cfg.HasMeta("File"))
            cfg.Save((string)cfg.GetMeta("File"));

        return @default;
    }

    #region Get

    public static T GetValue<[MustBeVariant] T>(this ConfigFile source, string key, T @default = default) => source.GetValue("", key, @default);
    public static T GetValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T @default = default)
        => source.HasSectionKey(section, key) ? source.GetValue(section, key).As<T>() : @default;

    public static T GetEnum<T>(this ConfigFile source, string key, T @default = default) where T : struct, Enum => source.GetEnum("", key, @default);
    public static T GetEnum<T>(this ConfigFile source, string section, string key, T @default = default) where T : struct, Enum
        => source.HasSectionKey(section, key) ? Enum.Parse<T>(source.GetValue(section, key).As<string>()) : @default;

    #endregion

    #region Set

    public static void SetValue<[MustBeVariant] T>(this ConfigFile source, string key, T value) => source.SetValue("", key, value);
    public static void SetValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T value)
        => source.SetValue(section, key, Variant.From(value));

    public static void SetEnum<T>(this ConfigFile source, string key, T value) where T : struct, Enum => source.SetEnum("", key, value);
    public static void SetEnum<T>(this ConfigFile source, string section, string key, T value) where T : struct, Enum
        => source.SetValue(section, key, $"{value}");

    #endregion
}
