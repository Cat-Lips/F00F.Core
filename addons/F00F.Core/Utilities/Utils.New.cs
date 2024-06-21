using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class New
{
    public static PlaneMesh PlaneMesh(int size, int lod, Material material)
    {
        Debug.Assert(size.IsPo2());

        lod = lod <= 0 ? size - 1
            : size / (int)Mathf.Pow(2, lod) - 1;

        return new()
        {
            Size = Vector2.One * size,
            SubdivideDepth = lod,
            SubdivideWidth = lod,
            Material = material,
        };

    }

    public static ShaderMaterial ShaderMaterial()
        => new() { Shader = new() };

    public static Texture2DArray Texture2DArray(params Texture2D[] textures)
    {
        if (textures?.Length is null or 0) return null;
        if (textures.Any(x => x is null)) return null;

        if (!TryGetImages(out var images))
            return null;

        var textureArray = new Texture2DArray();
        var err = textureArray.CreateFromImages(new(images.Select(x => x.Image)));
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
}
