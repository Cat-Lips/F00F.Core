using System.Diagnostics;
using System.Reflection;
using Godot;

namespace F00F
{
    public static partial class Utils
    {
        public static T GetScene<T>(this T _) where T : GodotObject => GetScene<T>();
        public static T GetScene<T>() where T : GodotObject => LoadScene<T>().Instantiate<T>();

        public static PackedScene LoadScene<T>(this T _) where T : GodotObject => LoadScene<T>();
        public static PackedScene LoadScene<T>() where T : GodotObject
            => Load<PackedScene>(GetScriptPath<T>().Replace(".cs", ".tscn"));

        public static Shader LoadShader<T>(this T _) where T : GodotObject => LoadShader<T>();
        public static Shader LoadShader<T>() where T : GodotObject
            => Load<Shader>(GetScriptPath<T>().Replace(".cs", ".gdshader"));

        public static Texture2D LoadTexture<T>(this T _, string relativePath) where T : GodotObject => LoadTexture<T>(relativePath);
        public static Texture2D LoadTexture<T>(string relativePath) where T : GodotObject
            => Load<Texture2D>(GetScriptDir<T>().PathJoin(relativePath));

        public static string GetScriptPath<T>() where T : GodotObject
            => typeof(T).GetCustomAttribute<ScriptPathAttribute>(false).Path;

        public static string GetScriptDir<T>() where T : GodotObject
            => GetScriptPath<T>().GetBaseDir();

        public static T Load<T>(string path) where T : class
        {
            Debug.Assert(ResourceLoader.Exists(path), $"Missing Resource: {path}!");
            return GD.Load<T>(path);
        }
    }
}
