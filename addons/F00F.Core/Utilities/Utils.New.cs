using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace F00F;

using Array = Godot.Collections.Array;

public static class New
{
    #region Mesh

    public static PlaneMesh PlaneMesh(int size, Material material = null) => PlaneMesh(Vector2I.One * size, material);
    public static PlaneMesh PlaneMesh(in Vector2I size, Material material = null) => new()
    {
        Size = size,
        Material = material,
        SubdivideWidth = size.X - 1,
        SubdivideDepth = size.Y - 1,
    };

    public static PlaneMesh PlaneMesh(int size, int lod, Material material = null) => PlaneMesh(size, out var _, lod, material);
    public static PlaneMesh PlaneMesh(int size, out int step, int lod, Material material = null)
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

    public static ArrayMesh ArrayMesh(int size, float amplitude, Func<float, float, float> RawHeight, Func<float, Color> RawColor = null, Func<float, float> RawGradient = null) => ArrayMesh(Vector2I.One * size, amplitude, RawHeight, RawColor, RawGradient);
    public static ArrayMesh ArrayMesh(in Vector2I size, float amplitude, Func<float, float, float> RawHeight, Func<float, Color> RawColor = null, Func<float, float> RawGradient = null)
    {
        var UseColor = RawColor is not null;
        RawGradient ??= _ => 1;

        var mesh = new ArrayMesh();
        var surface = GetSurfaceData(size);
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surface);
        if (UseColor) mesh.SurfaceSetMaterial(0, New.VertexColorMaterial());
        return mesh;

        Array GetSurfaceData(in Vector2I size)
        {
            var arrays = New.PlaneMesh(size).GetMeshArrays();
            var vertices = (Vector3[])arrays[(int)Mesh.ArrayType.Vertex];
            var normals = (Vector3[])arrays[(int)Mesh.ArrayType.Normal];
            var tangents = (float[])arrays[(int)Mesh.ArrayType.Tangent];
            var colors = UseColor ? new Color[vertices.Length] : null;

            for (var i = 0; i < vertices.Length; ++i)
            {
                var vertex = vertices[i];
                var height = RawHeight(vertex.X, vertex.Z);
                var normal = GetNormal(vertex.X, vertex.Z);
                var tangent = normal.Cross(Vector3.Up);

                vertex.Y = height * RawGradient(height) * amplitude;

                vertices[i] = vertex;
                normals[i] = normal;
                tangents[4 * i] = tangent.X;
                tangents[4 * i + 1] = tangent.Y;
                tangents[4 * i + 2] = tangent.Z;

                if (UseColor) colors[i] = RawColor(height);
            }

            arrays[(int)Mesh.ArrayType.Vertex] = vertices;
            arrays[(int)Mesh.ArrayType.Normal] = normals;
            arrays[(int)Mesh.ArrayType.Tangent] = tangents;
            if (UseColor) arrays[(int)Mesh.ArrayType.Color] = colors;

            return arrays;

            Vector3 GetNormal(float x, float z)
            {
                var e = Height(x + 1, z);
                var w = Height(x - 1, z);
                var n = Height(x, z + 1);
                var s = Height(x, z - 1);

                var dx = (e - w) * .5f;
                var dz = (n - s) * .5f;

                return new Vector3(dx, 1, dz).Normalized();

                float Height(float x, float z)
                {
                    var h = RawHeight(x, z);
                    var g = RawGradient(h);
                    return h * g * amplitude;
                }
            }
        }
    }

    #endregion

    #region Shapes

    public static HeightMapShape3D HeightMapShape(int size) => HeightMapShape(Vector2I.One * size);
    public static HeightMapShape3D HeightMapShape(in Vector2I size) => new()
    {
        MapWidth = size.X + 1,
        MapDepth = size.Y + 1,
    };

    public static HeightMapShape3D HeightMapShape(int size, in Vector2 center, Func<float, float, float> GetHeight) => HeightMapShape(Vector2I.One * size, center, GetHeight);
    public static HeightMapShape3D HeightMapShape(in Vector2I size, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var shape = HeightMapShape(size);
        shape.SetData(center, GetHeight);
        return shape;
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

    public static ConcavePolygonShape3D PolygonShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D PolygonShape(in Vector2I size) => new()
    {
        Data = PlaneMesh(size).GetFaces()
    };

    public static ConcavePolygonShape3D PolygonShape(int size, in Vector2 center, Func<float, float, float> GetHeight) => PolygonShape(Vector2I.One * size, center, GetHeight);
    public static ConcavePolygonShape3D PolygonShape(in Vector2I size, in Vector2 center, Func<float, float, float> GetHeight)
    {
        var shape = PolygonShape(size);
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

    public static ConcavePolygonShape3D StaticShape(in Vector2I size) => PolygonShape(size);
    public static ConcavePolygonShape3D ConcaveShape(in Vector2I size) => PolygonShape(size);
    public static ConcavePolygonShape3D TrimeshShape(in Vector2I size) => PolygonShape(size);

    public static ConcavePolygonShape3D StaticShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D ConcaveShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D TrimeshShape(int size) => PolygonShape(Vector2I.One * size);

    public static ConcavePolygonShape3D ConcavePlane(int size) => ConcavePlane(Vector2I.One * size);
    public static ConcavePolygonShape3D ConcavePlane(in Vector2I size) => new()
    {
        Data = new PlaneMesh { Size = size }.GetFaces(),
    };

    public static ConvexPolygonShape3D ConvexPlane(int size) => ConvexPlane(Vector2I.One * size);
    public static ConvexPolygonShape3D ConvexPlane(in Vector2I size) => new()
    {
        Points = (Vector3[])new PlaneMesh { Size = size }.GetMeshArrays()[(int)Mesh.ArrayType.Vertex],
    };

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

    public static StandardMaterial3D VertexColorMaterial()
        => new() { VertexColorUseAsAlbedo = true };

    #endregion

    #region Gradients

    public static Gradient Gradient(params Color[] colors)
    {
        var gradient = new Gradient { Colors = null, Offsets = null };

        var step = 1f / (colors.Length - 1);
        for (var i = 0; i < colors.Length; ++i)
            gradient.AddPoint(i * step, colors[i]);

        return gradient;
    }

    public static Gradient Gradient(params (float OffSet, Color Color)[] points)
    {
        var gradient = new Gradient { Colors = null, Offsets = null };

        foreach (var (offset, color) in points)
            gradient.AddPoint(offset, color);

        return gradient;
    }

    public static GradientTexture1D Texture(this Gradient gradient) => new()
    {
        Gradient = gradient,
    };

    #endregion

    #region Curves

    public static Curve Curve(params float[] points)
    {
        var curve = new Curve();
        var mode = Godot.Curve.TangentMode.Linear;

        var step = 1f / (points.Length - 1);
        for (var i = 0; i < points.Length; ++i)
            curve.AddPoint(new(i * step, points[i]), 0, 0, mode, mode);

        return curve;
    }

    public static Curve Curve(params (float x, float s)[] slope)
    {
        var curve = new Curve();
        var mode = Godot.Curve.TangentMode.Linear;

        var max = 0f;
        foreach (var (x, y) in Normalised([.. Points()]))
            curve.AddPoint(new(x, y), 0, 0, mode, mode);

        return curve;

        IEnumerable<(float x, float y)> Points()
        {
            var last = 0f;
            foreach (var (x, s) in slope)
            {
                var dx = x - last;
                max += s * dx;
                yield return (x, max);
                last = x;
            }
        }

        IEnumerable<(float x, float y)> Normalised(IEnumerable<(float x, float y)> pts)
        {
            foreach (var (x, y) in pts)
                yield return (x, y / max);
        }
    }

    public static CurveTexture Texture(this Curve curve) => new()
    {
        Curve = curve,
    };

    #endregion

    #region Noise

    public static FastNoiseLite Noise(bool smooth = false) => new()
    {
        NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
        FractalType = smooth ? FastNoiseLite.FractalTypeEnum.None : FastNoiseLite.FractalTypeEnum.Ridged,
    };

    public static NoiseTexture2D NoiseTexture(params Color[] colors) => NoiseTexture(512, null, colors);
    public static NoiseTexture2D NoiseTexture(bool smooth, params Color[] colors) => NoiseTexture(512, New.Noise(smooth), colors);
    public static NoiseTexture2D NoiseTexture(int size, params Color[] colors) => NoiseTexture(size, null, colors);
    public static NoiseTexture2D NoiseTexture(int size, bool smooth, params Color[] colors) => NoiseTexture(size, New.Noise(smooth), colors);
    public static NoiseTexture2D NoiseTexture(int size, Noise noise, params Color[] colors) => NoiseTexture(size, noise, false, colors);
    public static NoiseTexture2D NoiseTexture(int size, Noise noise, bool normalmap, params Color[] colors) => new()
    {
        Width = size,
        Height = size,
        Noise = noise ?? New.Noise(),
        ColorRamp = colors.IsNullOrEmpty() ? null : Gradient(colors),
        AsNormalMap = normalmap,
        GenerateMipmaps = false,
    };

    #endregion

    #region Utils

    public static T[] Array<T>(params T[] x) => x;

    #endregion

    #region Generators

    public static Vector3[] Loop(in Vector3 entry, in Vector3 exit, int resolution = 32) => Loop(entry, exit, Vector3.Up, resolution);
    public static Vector3[] Loop(in Vector3 entry, in Vector3 exit, in Vector3 up, int resolution = 32)
    {
        var chord = exit - entry;
        var length = chord.Length();
        var radius = length * .5f;

        var fwd = chord / length;
        var anchor = (entry + exit) * .5f;

        return Loop(anchor, fwd, up, radius, resolution);
    }

    public static Vector3[] Loop(in Vector3 anchor, in Vector3 fwd, float radius = .5f, int resolution = 32) => Loop(anchor, fwd, Vector3.Up, radius, resolution);
    public static Vector3[] Loop(in Vector3 anchor, float radius = .5f, int resolution = 32) => Loop(anchor, Vector3.Forward, Vector3.Up, radius, resolution);
    public static Vector3[] Loop(float radius = .5f, int resolution = 32) => Loop(Vector3.Zero, Vector3.Forward, Vector3.Up, radius, resolution);
    public static Vector3[] Loop(in Vector3 anchor, in Vector3 fwd, in Vector3 up, float radius = .5f, int resolution = 32)
    {
        var center = anchor + up * radius;
        var points = new Vector3[resolution];

        for (var i = 0; i < resolution; ++i)
        {
            var angle = Mathf.Tau * i / resolution - Mathf.Pi * .5f;
            points[i] = center +
                up * radius * Mathf.Sin(angle) +
                fwd * radius * Mathf.Cos(angle);
        }

        return points;
    }

    public static Mesh LoopMesh(Vector3[] points, in Vector3 up, float width = 1f, float thickness = .1f, float uScale = 1f, float vScale = 1f) => SweepMesh(points, Section(width, thickness), up, closed: true, uScale, vScale);
    public static Mesh LoopMesh(Vector3[] points, float width = 1f, float thickness = .1f, float uScale = 1f, float vScale = 1f) => SweepMesh(points, Vector3.Up, width, thickness, closed: true, uScale, vScale);
    public static Mesh LoopMesh(Vector3[] points, Vector3[] section, in Vector3 up, float uScale = 1f, float vScale = 1f) => SweepMesh(points, section, up, closed: true, uScale, vScale);

    public static Mesh SweepMesh(Vector3[] points, in Vector3 up, float width = 1f, float thickness = .1f, bool closed = false, float uScale = 1f, float vScale = 1f) => SweepMesh(points, Section(width, thickness), up, closed, uScale, vScale);
    public static Mesh SweepMesh(Vector3[] points, float width = 1f, float thickness = .1f, bool closed = false, float uScale = 1f, float vScale = 1f) => SweepMesh(points, Vector3.Up, width, thickness, closed, uScale, vScale);
    public static Mesh SweepMesh(Vector3[] points, Vector3[] section, in Vector3 up, bool closed = false, float uScale = 1f, float vScale = 1f)
    {
        var vcount = section.Length;
        var numSections = points.Length;
        var numVertices = numSections * vcount;
        var numIndicies = (closed ? numSections : numSections - 1) * vcount * 6;

        var vertices = new Vector3[numVertices];
        var normals = new Vector3[numVertices];
        var indicies = new int[numIndicies];
        var uvs = new Vector2[numVertices];

        CalcU(out var uTotal, out var uDistance);

        var idx = -1;
        var _up = up;
        for (var i = 0; i < numSections; ++i)
        {
            var first = i is 0;
            var last = i == numSections - 1;
            var next = last && !closed ? i - 1 : (i + 1) % numSections;

            var pos = points[i];
            var fwd = (points[next] - pos).Normalized();
            var right = fwd.Cross(_up).Normalized();
            _up = right.Cross(fwd).Normalized();
            var basis = new Basis(right, _up, fwd);

            var u = uDistance[i] / uTotal * uScale;

            var _idx = i * vcount;
            for (var j = 0; j < vcount; ++j)
            {
                var v = _idx + j;

                vertices[v] = pos + basis * section[j];
                normals[v] = _up;

                var vFrac = (float)j / (vcount - 1);
                uvs[v] = new Vector2(u, vFrac * vScale);
            }

            if (first && !closed) continue;

            var prev = (first ? numSections - 1 : i - 1) * vcount;
            for (var j = 0; j < vcount; ++j)
            {
                var a = prev + j;
                var b = prev + (j + 1) % vcount;
                var c = _idx + (j + 1) % vcount;
                var d = _idx + j;

                indicies[++idx] = a;
                indicies[++idx] = b;
                indicies[++idx] = c;

                indicies[++idx] = a;
                indicies[++idx] = c;
                indicies[++idx] = d;
            }
        }

        var arrays = new Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.Normal] = normals;
        arrays[(int)Mesh.ArrayType.Index] = indicies;
        arrays[(int)Mesh.ArrayType.TexUV] = uvs;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh;

        void CalcU(out float uTotal, out float[] uDistance)
        {
            uTotal = 0f;
            uDistance = new float[numSections];
            for (var i = 1; i < numSections; ++i)
                uDistance[i] = uTotal += points[i].DistanceTo(points[i - 1]);

            if (closed)
                uDistance[0] = uTotal += points[0].DistanceTo(points[numSections - 1]);
        }
    }

    private static Vector3[] Section(float width, float thickness) =>
    [
        Vector3.Left * width * .5f + Vector3.Down * thickness,
        Vector3.Right * width * .5f + Vector3.Down * thickness,
        Vector3.Right * width * .5f,
        Vector3.Left * width * .5f,
    ];

    public static ConcavePolygonShape3D StaticShape(Mesh mesh) => new() { Data = mesh.GetFaces() };
    public static ConcavePolygonShape3D ConcaveShape(Mesh mesh) => new() { Data = mesh.GetFaces() };
    public static ConcavePolygonShape3D TrimeshShape(Mesh mesh) => new() { Data = mesh.GetFaces() };

    #endregion
}
