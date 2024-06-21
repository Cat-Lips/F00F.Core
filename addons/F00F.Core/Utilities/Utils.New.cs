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

    public static ArrayMesh TerrainMesh(int size, float amplitude, Func<float, float, float> RawHeight, Func<float, Color> RawColor = null, Func<float, float> RawGradient = null) => TerrainMesh(Vector2I.One * size, amplitude, RawHeight, RawColor, RawGradient);
    public static ArrayMesh TerrainMesh(in Vector2I size, float amplitude, Func<float, float, float> RawHeight, Func<float, Color> RawColor = null, Func<float, float> RawGradient = null)
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

    public static ConcavePolygonShape3D StaticShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D ConcaveShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D TrimeshShape(int size) => PolygonShape(Vector2I.One * size);
    public static ConcavePolygonShape3D PolygonShape(int size) => PolygonShape(Vector2I.One * size);

    public static ConcavePolygonShape3D StaticShape(in Vector2I size) => PolygonShape(size);
    public static ConcavePolygonShape3D ConcaveShape(in Vector2I size) => PolygonShape(size);
    public static ConcavePolygonShape3D TrimeshShape(in Vector2I size) => PolygonShape(size);
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

    public static ConcavePolygonShape3D StaticShape(Mesh mesh) => new() { Data = mesh.GetFaces() };
    public static ConcavePolygonShape3D ConcaveShape(Mesh mesh) => new() { Data = mesh.GetFaces() };
    public static ConcavePolygonShape3D TrimeshShape(Mesh mesh) => new() { Data = mesh.GetFaces() };

    //public static ConvexPolygonShape3D ConvexShape(Mesh mesh) => new() { Points = [.. mesh.GetFaces().Distinct()] };
    public static ConvexPolygonShape3D ConvexShape(Mesh mesh, int surface = 0) => new() { Points = mesh.Vertices(surface) };
    private static Vector3[] Vertices(this Mesh mesh, int surface = 0) => [.. ((Vector3[])mesh.SurfaceGetArrays(surface)[(int)Mesh.ArrayType.Vertex]).Distinct()];

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

    public static Curve Curve(params IEnumerable<Vector2> points)
    {
        var curve = new Curve();
        points.ForEach(p => curve.AddPoint(p));
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

    public static Mesh StuntLoopMesh(float radius = 1f, int segments = 64, float width = .5f, float thickness = .1f, float gap = 0f, bool flip = false)
    {
        var halfWidth = width * .5f;
        var halfOffset = (width + gap) * .5f * (flip ? -1 : 1);

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var indices = new List<int>();
        var uvs = new List<Vector2>();

        AddVertices();
        SetIndicies();
        AddEndCaps();

        return ArrayMesh(vertices, normals, indices, uvs);

        void AddVertices()
        {
            var curV = 0f;
            var curUp = Vector3.Up;
            var curRight = Vector3.Right;
            var curCenter = Vector3.Zero;

            for (var i = 0; i <= segments; ++i)
            {
                Update(i);
                AddQuad();
            }

            void Update(int i)
            {
                var t = i / (float)segments;

                var angle = Mathf.Tau * t;
                var cosAngle = Mathf.Cos(angle);
                var sinAngle = Mathf.Sin(angle);

                var loopY = radius - radius * cosAngle;
                var loopZ = radius * sinAngle;

                var bankAngle = Mathf.Pi * t;
                var lateralShift = -halfOffset * Mathf.Cos(bankAngle);
                var center = new Vector3(lateralShift, loopY, loopZ);

                var tangent = new Vector3(0, sinAngle, cosAngle);
                curRight = curUp.Cross(tangent).Normalized();
                curUp = tangent.Cross(curRight).Normalized();

                curV += center.DistanceTo(curCenter) / width;
                curCenter = center;
            }

            void AddQuad()
            {
                var v1 = curCenter - curRight * halfWidth;
                var v2 = curCenter + curRight * halfWidth;
                var height = curUp * thickness;

                Top();
                Bottom();
                Left();
                Right();

                void Top()
                {
                    vertices.Add(v1);
                    vertices.Add(v2);
                    normals.Add(curUp);
                    normals.Add(curUp);
                    uvs.Add(new(0, curV));
                    uvs.Add(new(1, curV));
                }

                void Bottom()
                {
                    vertices.Add(v1 - height);
                    vertices.Add(v2 - height);
                    normals.Add(-curUp);
                    normals.Add(-curUp);
                    uvs.Add(new(2, curV));
                    uvs.Add(new(3, curV));
                }

                void Left()
                {
                    vertices.Add(v1);
                    vertices.Add(v1 - height);
                    normals.Add(-curRight);
                    normals.Add(-curRight);
                    uvs.Add(new(1, curV));
                    uvs.Add(new(2, curV));
                }

                void Right()
                {
                    vertices.Add(v2);
                    vertices.Add(v2 - height);
                    normals.Add(curRight);
                    normals.Add(curRight);
                    uvs.Add(new(3, curV));
                    uvs.Add(new(4, curV));
                }
            }
        }

        void SetIndicies()
        {
            var totalQuads = vertices.Count / 8 - 1;
            for (var i = 0; i < totalQuads; ++i)
            {
                var idx = i * 8;
                SetTop(idx);
                SetBottom(idx);
                SetLeft(idx);
                SetRight(idx);
            }

            void SetTop(int idx)
            {
                indices.AddAntiClockwise(
                    idx,
                    idx + 1,
                    idx + 8,
                    idx + 9);
            }

            void SetBottom(int idx)
            {
                indices.AddClockwise(
                    idx + 2,
                    idx + 3,
                    idx + 10,
                    idx + 11);
            }

            void SetLeft(int idx)
            {
                indices.AddClockwise(
                    idx + 4,
                    idx + 5,
                    idx + 12,
                    idx + 13);
            }

            void SetRight(int idx)
            {
                indices.AddAntiClockwise(
                    idx + 6,
                    idx + 7,
                    idx + 14,
                    idx + 15);
            }
        }

        void AddEndCaps()
        {
            var up = Vector3.Up;
            var right = Vector3.Right;
            var height = up * thickness;

            AddCap(flip);
            AddCap(!flip);

            void AddCap(bool fwd)
            {
                var side = fwd ? -1 : 1;

                var center = new Vector3(halfOffset * side, 0, 0);
                var normal = new Vector3(0, 0, side);

                var v1 = center - right * halfWidth;
                var v2 = center + right * halfWidth;

                var idx = vertices.Count;
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v1 - height);
                vertices.Add(v2 - height);

                for (var i = 0; i < 4; ++i)
                {
                    normals.Add(normal);
                    uvs.Add(new Vector2(i % 2, i / 2));
                }

                indices.Add(fwd, idx);
            }
        }
    }

    #region Utils

    private static void Add(this List<int> indices, bool fwd, int idx)
    {
        if (fwd) indices.AddClockwise(idx);
        else indices.AddAntiClockwise(idx);
    }

    private static void Add(this List<int> indices, bool fwd, int a, int b, int c, int d)
    {
        if (fwd) indices.AddClockwise(a, b, c, d);
        else indices.AddAntiClockwise(a, b, c, d);
    }

    private static void AddClockwise(this List<int> indices, int idx)
        => indices.AddClockwise(idx, idx + 1, idx + 2, idx + 3);

    private static void AddAntiClockwise(this List<int> indices, int idx)
        => indices.AddAntiClockwise(idx, idx + 1, idx + 2, idx + 3);

    private static void AddClockwise(this List<int> indices, int a, int b, int c, int d)
    {
        indices.Add(a);
        indices.Add(c);
        indices.Add(b);
        indices.Add(b);
        indices.Add(c);
        indices.Add(d);
    }

    private static void AddAntiClockwise(this List<int> indices, int a, int b, int c, int d)
    {
        indices.Add(a);
        indices.Add(b);
        indices.Add(c);
        indices.Add(b);
        indices.Add(d);
        indices.Add(c);
    }

    private static ArrayMesh ArrayMesh(List<Vector3> vertices, List<Vector3> normals, List<int> indices, List<Vector2> uvs)
        => ArrayMesh(vertices.ToArray(), normals.ToArray(), indices.ToArray(), uvs.ToArray());

    private static ArrayMesh ArrayMesh(Vector3[] vertices, Vector3[] normals, int[] indices, Vector2[] uvs)
    {
        var surfaceArray = new Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);
        surfaceArray[(int)Mesh.ArrayType.Vertex] = vertices;
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals;
        surfaceArray[(int)Mesh.ArrayType.Index] = indices;
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        return mesh;
    }

    #endregion

    #endregion
}
