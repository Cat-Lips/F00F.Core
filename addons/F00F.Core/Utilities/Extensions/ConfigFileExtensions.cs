using System;
using System.Diagnostics;
using Godot;

namespace F00F;

public static class ConfigFileExtensions
{
    #region Load

    public static ConfigFile LoadConfig(this string path, bool autoSave = true)
    {
        var cfg = new ConfigFile();

        if (autoSave)
            cfg.SetMeta("File", path);

        if (FS.FileExists(path))
            cfg.Load(path);

        return cfg;
    }

    #endregion

    #region Get

    public static T GetV<[MustBeVariant] T>(this ConfigFile cfg, string key, T @default = default) => cfg.GetV("", key, @default);
    public static T GetV<[MustBeVariant] T>(this ConfigFile cfg, string section, string key, T @default = default)
    {
        Debug.Assert(!typeof(T).IsEnum);

        if (cfg.HasSectionKey(section, key))
            return cfg.GetValue(section, key).As<T>();

        cfg.SetV(section, key, @default);

        if (cfg.HasMeta("File"))
            cfg.Save((string)cfg.GetMeta("File"));

        return @default;
    }

    public static T GetE<T>(this ConfigFile cfg, string key, T @default = default) where T : struct, Enum => cfg.GetE("", key, @default);
    public static T GetE<T>(this ConfigFile cfg, string section, string key, T @default = default) where T : struct, Enum
    {
        Debug.Assert(typeof(T).IsEnum);

        if (cfg.HasSectionKey(section, key))
            return Enum.Parse<T>((string)cfg.GetValue(section, key));

        cfg.SetE(section, key, @default);

        if (cfg.HasMeta("File"))
            cfg.Save((string)cfg.GetMeta("File"));

        return @default;
    }

    #endregion

    #region Set

    public static void SetV<[MustBeVariant] T>(this ConfigFile source, string key, T value) => source.SetV("", key, value);
    public static void SetV<[MustBeVariant] T>(this ConfigFile source, string section, string key, T value)
    {
        Debug.Assert(!typeof(T).IsEnum);
        source.SetValue(section, key, Variant.From(value));
    }

    public static void SetE<T>(this ConfigFile source, string key, T value) where T : struct, Enum => source.SetE("", key, value);
    public static void SetE<T>(this ConfigFile source, string section, string key, T value) where T : struct, Enum
    {
        Debug.Assert(typeof(T).IsEnum);
        source.SetValue(section, key, $"{value}");
    }

    #endregion
}
