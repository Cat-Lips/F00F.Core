using System;
using Godot;

using static Godot.Mesh;
using Array = Godot.Collections.Array;
using CT = System.Threading.CancellationToken;
using CTS = System.Threading.CancellationTokenSource;

namespace F00F;

public static class MeshExtensions_Async
{
    public static void SetMeshAsync(this MeshInstance3D source, ref CTS cts, int size, int lod, Func<float, float, int, (float Height, float Raw)> GetHeight, Func<float, Color> GetColor, Action Ready = null, bool batch = true)
    {
        source.RunAsync(ref cts, CreateMesh, OnMeshReady);

        ArrayMesh CreateMesh(CT ct)
        {
            var mesh = new ArrayMesh();
            var surface = SurfaceData();
            if (ct.Cancelled()) return null;
            mesh.AddSurfaceFromArrays(PrimitiveType.Triangles, surface);
            return mesh;

            Array SurfaceData()
            {
                var arrays = New.PlaneMesh(size, out var step, lod).GetMeshArrays();
                var vertices = (Vector3[])arrays[(int)ArrayType.Vertex];
                var normals = (Vector3[])arrays[(int)ArrayType.Normal];
                var tangents = (float[])arrays[(int)ArrayType.Tangent];
                var colors = new Color[vertices.Length];
                var dblStep = step * 2;

                for (var i = 0; i < vertices.Length; ++i)
                {
                    if (ct.Cancelled()) return null;

                    var vertex = vertices[i];
                    var (height, raw) = GetHeight(vertex.X, vertex.Z, step);
                    var normal = GetNormal(vertex.X, vertex.Z);
                    var tangent = normal.Cross(Vector3.Up);

                    vertex.Y = height;

                    vertices[i] = vertex;
                    normals[i] = normal;
                    tangents[4 * i] = tangent.X;
                    tangents[4 * i + 1] = tangent.Y;
                    tangents[4 * i + 2] = tangent.Z;
                    colors[i] = GetColor(raw);
                }

                arrays[(int)ArrayType.Vertex] = vertices;
                arrays[(int)ArrayType.Normal] = normals;
                arrays[(int)ArrayType.Tangent] = tangents;
                arrays[(int)ArrayType.Color] = colors;
                return ct.Cancelled() ? null : arrays;

                Vector3 GetNormal(float x, float z)
                {
                    var e = GetHeight(x + step, z, step).Height;
                    var w = GetHeight(x - step, z, step).Height;
                    var n = GetHeight(x, z + step, step).Height;
                    var s = GetHeight(x, z - step, step).Height;

                    var dx = (e - w) / dblStep;
                    var dz = (n - s) / dblStep;

                    return new Vector3(dx, 1, dz).Normalized();
                }
            }
        }

        void OnMeshReady(ArrayMesh mesh)
        {
            mesh.SurfaceSetMaterial(0, New.VertexColorMaterial());

            source.Mesh = mesh;
            Ready?.Invoke();
        }
    }
}
