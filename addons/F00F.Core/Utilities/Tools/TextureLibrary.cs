using Godot;

namespace F00F;

[Tool, GlobalClass]
public partial class TextureLibrary : CustomResource
{
    public void Set(ShaderMaterial shader)
    {
        //if (Textures is null)
        //{
        //    ShaderMaterial.SetShaderParameter("albedo_array", default);
        //    ShaderMaterial.SetShaderParameter("normal_array", default);
        //    ShaderMaterial.SetShaderParameter("height_array", default);
        //    ShaderMaterial.SetShaderParameter("metallic_array", default);
        //    ShaderMaterial.SetShaderParameter("roughness_array", default);
        //    ShaderMaterial.SetShaderParameter("occlusion_array", default);
        //}
        //else
        //{
        //    ShaderMaterial.SetShaderParameter("albedo_array", Textures.Albedo);
        //    ShaderMaterial.SetShaderParameter("normal_array", Textures.Normal);
        //    ShaderMaterial.SetShaderParameter("height_array", Textures.Height);
        //    ShaderMaterial.SetShaderParameter("metallic_array", Textures.Metallic);
        //    ShaderMaterial.SetShaderParameter("roughness_array", Textures.Roughness);
        //    ShaderMaterial.SetShaderParameter("occlusion_array", Textures.Occlusion);
        //}
    }
}

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Godot;

//namespace F00F;

//[Tool, GlobalClass]
//public partial class TextureLibrary : TextureSet
//{
//    #region Private

//    private readonly SortedDictionary<string, Dictionary<string, string>> lib = [];

//    #endregion

//    public event Action KeyChanged;
//    public event Action KeysChanged;

//    #region Export

//    [Export(PropertyHint.Dir)] public string ResDir { get; set => this.Set(ref field, value, Reset); }
//    [Export] public string Key { get; set => this.Set(ref field, Validate(value), notify: true, OnKeyChanged, KeyChanged); }
//    [Export] public string[] Keys { get; private set => this.Set(ref field, value, notify: true, KeysChanged); }

//    #endregion

//    public void Reset()
//    {
//        DisableChangedEvent();
//        Load();
//        EnableChangedEvent();

//        void Load()
//        {
//            lib.Clear();

//            Albedo = null;
//            Normal = null;
//            Height = null;
//            Metallic = null;
//            Occlusion = null;

//            ResourceLoader.ListDirectory(ResDir).ForEach(file =>
//            {
//                if (file.EndsWith('/')) return;

//                var parts = Path.GetFileNameWithoutExtension(file).SafeCapitalise().Split();
//                if (parts.Length < 2) return;

//                var group = string.Join(" ", parts.SkipLast(1));
//                var token = parts.Last().ToLowerInvariant();

//                if (TOKEN.Albedo.Contains(token)) Add(group, KEY.Albedo, ResDir.PathJoin(file));
//                else if (TOKEN.Normal.Contains(token)) Add(group, KEY.Normal, ResDir.PathJoin(file));
//                else if (TOKEN.Height.Contains(token)) Add(group, KEY.Height, ResDir.PathJoin(file));
//                else if (TOKEN.Metallic.Contains(token)) Add(group, KEY.Metallic, ResDir.PathJoin(file));
//                else if (TOKEN.Occlusion.Contains(token)) Add(group, KEY.Occlusion, ResDir.PathJoin(file));
//            });

//            var oldKey = Key;
//            EnableChangedEvent();
//            Keys = [.. lib.Keys];
//            DisableChangedEvent();
//            if (Key == oldKey)
//            {
//                if (IsValidKey(oldKey))
//                    OnKeyChanged();
//                else Key = Keys.PickRandom();
//            }

//            void Add(string group, string key, string path)
//            {
//                if (!lib.TryGetValue(group, out var set))
//                    lib.Add(group, set = []);
//                set.Add(key, path);
//            }
//        }
//    }

//    public bool IsValidKey(string key)
//        => key.NotNull() && lib.ContainsKey(key);

//    #region Private

//    private string Validate(string key)
//        => IsValidKey(key) ? key : Keys.PickRandom();

//    private void OnKeyChanged()
//    {
//        if (Key.NotNull() && lib.TryGetValue(Key, out var group))
//        {
//            Albedo = TryGetTexture(KEY.Albedo);
//            Normal = TryGetTexture(KEY.Normal);
//            Height = TryGetTexture(KEY.Height);
//            Metallic = TryGetTexture(KEY.Metallic);
//            Occlusion = TryGetTexture(KEY.Occlusion);
//        }
//        else
//        {
//            Albedo = null;
//            Normal = null;
//            Height = null;
//            Metallic = null;
//            Occlusion = null;
//        }

//        Texture2D TryGetTexture(string key)
//            => group.TryGetValue(key, out var path) ? Utils.Load<Texture2D>(path) : null;
//    }

//    private static class KEY
//    {
//        public const string Albedo = "Albedo";
//        public const string Normal = "Normal";
//        public const string Height = "Height";
//        public const string Metallic = "Metallic";
//        public const string Occlusion = "Occlusion";
//    }

//    private static class TOKEN
//    {
//        public static readonly HashSet<string> Albedo = ["albedo", "diffuse", "colour", "color", "base", "basecolour", "basecolor"];
//        public static readonly HashSet<string> Normal = ["normal", "norm", "nml"];
//        public static readonly HashSet<string> Height = ["height", "displacement", "disp", "bump"];
//        public static readonly HashSet<string> Metallic = ["metallic", "metalness", "metal"];
//        public static readonly HashSet<string> Occlusion = ["occlusion", "ambient", "ambience", "ambiance", "ambientocclusion", "ao"];
//    }

//#if TOOLS

//    public override void _ValidateProperty(Godot.Collections.Dictionary property)
//    {
//        if (Editor.SetReadOnly(property, PropertyName.Keys)) return;
//        if (Editor.SetEnumHint(property, PropertyName.Key, Keys)) return;
//    }

//#endif

//    #endregion
//}
