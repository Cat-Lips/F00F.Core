using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;

namespace F00F;

public static partial class Utils
{
    public static Node New(this PackedScene scene) => scene.New<Node>();
    public static T New<T>(this PackedScene scene) where T : Node => (T)scene.Instantiate();

    public static Node New(this PackedScene scene, Action<Node> init) => scene.New<Node>(init);
    public static T New<T>(this PackedScene scene, Action<T> init) where T : Node { var x = scene.New<T>(); init(x); return x; }

    public static T New<T>() where T : Node => LoadScene<T>().New<T>();
    public static T New<T>(Action<T> init) where T : Node => LoadScene<T>().New(init);

    public static T New<T>(this T _) where T : Node => New<T>();
    public static T New<T>(this T _, Action<T> init) where T : Node => New(init);

    public static PackedScene LoadScene<T>(this T _) where T : Node => LoadScene<T>();
    public static PackedScene LoadScene<T>() where T : Node
        => Load<PackedScene>(GetScriptPath<T>(".tscn"));

    public static Shader LoadShader<T>(this T _) where T : GodotObject => LoadShader<T>();
    public static Shader LoadShader<T>() where T : GodotObject
        => Load<Shader>(GetScriptPath<T>(".gdshader"));

    public static Texture2D LoadTexture<T>(this T _, string relativePath) where T : GodotObject => LoadTexture<T>(relativePath);
    public static Texture2D LoadTexture<T>(string relativePath) where T : GodotObject
        => Load<Texture2D>(GetScriptDir<T>().PathJoin(relativePath));

    public static ConfigFile LoadConfig<T>(this T _, bool autoSave = true) where T : GodotObject => LoadConfig<T>(autoSave);
    public static ConfigFile LoadConfig<T>(bool autoSave = true) where T : GodotObject
        => GetScriptPath<T>(".cfg").LoadConfig(autoSave);

    public static T LoadRes<T>(this T _) where T : Resource => LoadRes<T>();
    public static T LoadRes<T>() where T : Resource
        => Load<T>(GetScriptPath<T>(".tres"));

    public static string GetScriptPath<T>(string ext) where T : GodotObject
        => Path.ChangeExtension(GetScriptPath<T>(), ext);

    public static string GetScriptPath<T>() where T : GodotObject
        => typeof(T).GetCustomAttribute<ScriptPathAttribute>(false).Path;

    public static string GetScriptDir<T>() where T : GodotObject
        => GetScriptPath<T>().GetBaseDir();

    public static T Load<T>(string path) where T : class
    {
        Debug.Assert(ResourceLoader.Exists(path), $"Missing Resource: {path}!");
        return GD.Load<T>(path);
    }

    public static T New<T>(this ResourcePreloader preload) where T : Node
        => (T)preload.Get<T>().Instantiate();

    public static PackedScene Get<T>(this ResourcePreloader preload)
        => preload.Get<PackedScene>(typeof(T).Name);

    public static T Get<T>(this ResourcePreloader preload, StringName name) where T : Resource
        => (T)preload.GetResource(name);

    public static IEnumerable<PackedScene> Get(this ResourcePreloader preload)
        => preload.GetResourceList().Select(x => preload.Get<PackedScene>(x));
}
