﻿using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

public static class New
{
    #region Mesh

    public static PlaneMesh PlaneMesh(int size, int lod = 0, Material material = null)
        => PlaneMesh(size, out var _, lod, material);

    public static PlaneMesh PlaneMesh(int size, out int step, int lod = 0, Material material = null)
    {
        var div = Div();
        step = size / (div + 1);
        return new()
        {
            Size = Vector2.One * size,
            SubdivideDepth = div,
            SubdivideWidth = div,
            Material = material,
        };

        int Div()
        {
            Debug.Assert(lod <= 0 || size.IsPo2());

            var div = lod <= 0 ? size - 1
                : size / (int)Mathf.Pow(2, lod) - 1;

            return div.ClampMin(0);
        }
    }

    #endregion

    #region Shapes

    public static ConcavePolygonShape3D PolygonShape(int size) => new()
    {
        Data = PlaneMesh(size).GetFaces()
    };

    public static HeightMapShape3D HeightMapShape(int size) => new()
    {
        MapWidth = size + 1,
        MapDepth = size + 1,
    };

    public static ConcavePolygonShape3D PolygonShape(int size, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var shape = PolygonShape(size);
        shape.SetData(center, GetHeight);
        return shape;
    }

    public static HeightMapShape3D HeightMapShape(int size, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var shape = HeightMapShape(size);
        shape.SetData(center, GetHeight);
        return shape;
    }

    public static void SetData(this ConcavePolygonShape3D source, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var data = source.Data;
        SetData(center);
        source.Data = data;

        void SetData(in Vector2 center)
        {
            var origin = center.FromXZ();
            for (var i = 0; i < data.Length; ++i)
            {
                var vertex = origin + data[i];
                data[i].Y = GetHeight(vertex.X, vertex.Z);
            }
        }
    }

    public static void SetData(this HeightMapShape3D source, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var data = source.MapData;
        SetData(center, source.MapWidth, source.MapDepth);
        source.MapData = data;

        void SetData(in Vector2 center, int width, int depth)
        {
            var x0 = center.X - (width - 1) * .5f;
            var z0 = center.Y - (depth - 1) * .5f;
            for (var x = 0; x < width; ++x)
            {
                for (var z = 0; z < depth; ++z)
                {
                    var i = x + z * width;
                    data[i] = GetHeight(x0 + x, z0 + z);
                }
            }
        }
    }

    #endregion

    #region Shaders

    public static ShaderMaterial ShaderAsMaterial<T>() where T : GodotObject
        => ShaderMaterial(Utils.LoadShader<T>());

    public static ShaderMaterial ShaderMaterial<T>() where T : GodotObject
        => ShaderMaterial(Utils.LoadShader<T>());

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

    #region Materials

    public static StandardMaterial3D Material(in Color color)
        => new() { AlbedoColor = color };

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

    #region Utils

    public static T[] Array<T>(params T[] x) => x;

    #endregion
}
