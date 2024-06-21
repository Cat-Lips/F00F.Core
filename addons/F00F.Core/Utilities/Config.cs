using System.Linq;
using Godot;

namespace F00F;

public class Config<T>(bool autoSave = true) : Config(typeof(T).Name, autoSave) { }

public class Config
{
    private static readonly string UserConfigDir = "Settings";

    private readonly ConfigFile config = new();

    private readonly string path;
    private readonly bool autosave;

    public Config(string name, bool autosave = true)
    {
        path = $"user://{UserConfigDir}/{name}.cfg";
        if (this.autosave = autosave)
            FS.MakeDir(path.GetBaseDir());

        if (FS.FileExists(path))
            Load();
    }

    public void Set<[MustBeVariant] T>(string key, T value) => Set("", key, value);
    public void Set<[MustBeVariant] T>(string section, string key, T value)
    {
        config.SetValue(section, key, Variant.From(value));
        if (autosave) Save();
    }

    public T Get<[MustBeVariant] T>(string key) => Get<T>("", key);
    public T Get<[MustBeVariant] T>(string section, string key)
        => TryGet<T>(section, key, out var value) ? value : value;

    public T Get<[MustBeVariant] T>(string key, T @default) => Get("", key, @default);
    public T Get<[MustBeVariant] T>(string section, string key, T @default)
        => TryGet<T>(section, key, out var value) ? value : @default;

    public bool TryGet<[MustBeVariant] T>(string key, out T value) => TryGet("", key, out value);
    public bool TryGet<[MustBeVariant] T>(string section, string key, out T value)
    {
        if (config.HasSectionKey(section, key))
        {
            value = config.GetValue(section, key).As<T>();
            return true;
        }

        value = default;
        return false;
    }

#if DEBUG
    public void Load()
        => config.Load(path);

    public void Save()
        => config.Save(path);
#else
    public void Load()
        => config.LoadEncryptedPass(path, App.Name);

    public void Save()
        => config.SaveEncryptedPass(path, App.Name);
#endif

    public void Clear() => config.Clear();
    public void Clear(string key) => Clear("", key);
    public void Clear(string section, string key)
    {
        if (config.HasSectionKey(section, key))
            config.EraseSectionKey(section, key);
        if (config.GetSectionKeys(section).Length is 0)
            config.EraseSection(section);
        if (autosave) Save();
    }

    public string[] Keys() => Keys("");
    public string[] Sections() => config.GetSections().Except([""]).ToArray();
    public string[] Keys(string section) => config.HasSection(section) ? config.GetSectionKeys(section) : [];
}
