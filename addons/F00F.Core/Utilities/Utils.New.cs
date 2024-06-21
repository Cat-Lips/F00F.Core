using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class New
{
    #region Mesh

    public static PlaneMesh PlaneMesh(int size, int lod = 0, Material material = null)
        => NewPlaneMesh(size, Div(size, lod), material);

    public static PlaneMesh PlaneMesh(out int div, int size, int lod = 0, Material material = null)
        => NewPlaneMesh(size, div = Div(size, lod), material);

    private static PlaneMesh NewPlaneMesh(int size, int div, Material material)
    {
        return new()
        {
            Size = Vector2.One * size,
            SubdivideDepth = div,
            SubdivideWidth = div,
            Material = material,
        };
    }

    #endregion

    #region Shapes

    public static ConcavePolygonShape3D TrimeshShape(int size) => new()
    {
        Data = PlaneMesh(size).GetFaces()
    };

    public static ConvexPolygonShape3D PolygonShape(int size) => new()
    {
        Points = PlaneMesh(size).GetFaces()
    };

    public static HeightMapShape3D HeightMapShape(int size) => new()
    {
        MapWidth = size,
        MapDepth = size,
    };

    #endregion

    #region Shaders

    public static ShaderMaterial Material(Shader shader = null)
        => new() { Shader = shader ?? new() };

    [Obsolete("Use New.Material instead")]
    public static ShaderMaterial ShaderMaterial(Shader shader = null)
        => new() { Shader = shader ?? new() };

    public static Texture2DArray Texture2DArray(params Texture2D[] textures)
    {
        if (textures?.Length is null or 0) return null;
        if (textures.Any(x => x is null)) return null;

        if (!TryGetImages(out var images))
            return null;

        var textureArray = new Texture2DArray();
        var err = textureArray.CreateFromImages([.. images.Select(x => x.Image)]);
        if (err.NotOk()) { GD.PushWarning($"Error creating texture array [{err}]"); return null; }
        return textureArray;

        bool TryGetImages(out (string File, Image Image)[] images)
        {
            return ValidateImages(images = textures.Select(x => (File: x.ResourcePath.GetFile(), Image: x.GetImage())).ToArray());

            static bool ValidateImages(in (string File, Image Image)[] images)
            {
                return NoNullImages(images) && MatchingState(images);

                bool NoNullImages(in (string File, Image Image)[] images)
                {
                    var nullImages = images.ToLookup(x => x.Image is null)[true].ToArray();
                    if (nullImages.Length is 0) return true;
                    nullImages.ForEach(x => GD.PushWarning($"Null Image: {x.File}"));
                    return false;
                }

                bool MatchingState(in (string File, Image Image)[] images)
                {
                    var first = images.First().Image;
                    var width = first.GetWidth();
                    var height = first.GetHeight();
                    var format = first.GetFormat();
                    var mipmap = first.HasMipmaps();
                    Debug.Assert(!mipmap);

                    var valid = true;
                    images.Skip(1).ForEach(ValidateState);
                    return valid;

                    void ValidateState((string File, Image Image) x)
                    {
                        var myWidth = x.Image.GetWidth();
                        var myHeight = x.Image.GetHeight();
                        var myFormat = x.Image.GetFormat();
                        var myMipmap = x.Image.HasMipmaps();
                        Debug.Assert(!myMipmap);

                        if (width != myWidth) SetError($"Width Mismatch: {x.File} [Expected {width}, Actual: {myWidth}]");
                        if (height != myHeight) SetError($"Height Mismatch: {x.File} [Expected {height}, Actual: {myHeight}]");
                        if (format != myFormat) SetError($"Format Mismatch: {x.File} [Expected {format}, Actual: {myFormat}]");
                        if (mipmap != myMipmap) SetError($"Mipmap Mismatch: {x.File} [Expected {mipmap}, Actual: {myMipmap}]");

                        void SetError(string msg)
                        {
                            valid = false;
                            GD.PushWarning(msg);
                        }
                    }
                }
            }
        }
    }

    public static Texture2D Texture1D(params Color[] colors)
    {
        if (colors?.Length is null or 0) return null;

        var img = Image.CreateEmpty(colors.Length, 1, false, Image.Format.Rgba8);
        for (var i = 0; i < colors.Length; ++i) img.SetPixel(i, 0, colors[i]);
        return ImageTexture.CreateFromImage(img);
    }

    public static Texture2D Texture1D(params float[] values)
    {
        if (values?.Length is null or 0) return null;

        var img = Image.CreateEmpty(values.Length, 1, false, Image.Format.Rf);
        for (var i = 0; i < values.Length; ++i) img.SetPixel(i, 0, new Color(values[i], 0, 0));
        return ImageTexture.CreateFromImage(img);
    }

    #endregion

    #region Noise

    public static FastNoiseLite Noise() => new()
    {
        NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
        FractalType = FastNoiseLite.FractalTypeEnum.Ridged,
    };

    public static Gradient Gradient(params Color[] colors) => new()
    {
        Colors = colors,
    };

    public static NoiseTexture2D NoiseTexture(params Color[] colors) => NoiseTexture(512, null, colors);
    public static NoiseTexture2D NoiseTexture(int size, params Color[] colors) => NoiseTexture(size, null, colors);
    public static NoiseTexture2D NoiseTexture(int size, Noise noise, params Color[] colors) => NoiseTexture(size, noise, false, colors);
    public static NoiseTexture2D NoiseTexture(int size, Noise noise, bool normalmap, params Color[] colors) => new()
    {
        Width = size,
        Height = size,
        Noise = noise ?? Noise(),
        ColorRamp = colors.Length is 0 ? null : Gradient(colors),
        AsNormalMap = normalmap,
        GenerateMipmaps = false,
    };

    #endregion

    #region Private

    private static int Div(int size, int lod)
    {
        Debug.Assert(lod <= 0 || size.IsPo2());

        var div = lod <= 0 ? size - 1
            : size / (int)Mathf.Pow(2, lod) - 1;

        return div.ClampMin(0);
    }

    #endregion
}
