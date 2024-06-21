using System;
using Godot;
using static Godot.Mesh;
using Array = Godot.Collections.Array;
using CT = System.Threading.CancellationToken;
using CTS = System.Threading.CancellationTokenSource;

namespace F00F;

public static class MeshExtensions_Mesh
{
    public static void SetMeshAsync(this MeshInstance3D source, ref CTS cts, int size, int lod, Func<float, float, float> GetHeight, Action Ready = null)
    {
        source.RunAsync(ref cts, CreateMesh, OnMeshReady);

        ArrayMesh CreateMesh(CT ct)
        {
            var mesh = new ArrayMesh();
            mesh.AddSurfaceFromArrays(PrimitiveType.Triangles, SurfaceData());
            return mesh;

            Array SurfaceData()
            {
                var arrays = New.PlaneMesh(out var div, size, lod).GetMeshArrays();
                var vertices = (Vector3[])arrays[(int)ArrayType.Vertex];
                var normals = (Vector3[])arrays[(int)ArrayType.Normal];
                var tangents = (float[])arrays[(int)ArrayType.Tangent];

                var epsilon = size / div.ClampMin(1);
                var epsilon2 = epsilon * 2f;

                for (var i = 0; i < vertices.Length; ++i)
                {
                    if (ct.Cancelled()) return null;

                    var vertex = vertices[i];
                    vertex.Y = GetHeight(vertex.X, vertex.Z);
                    var normal = GetNormal(vertex.X, vertex.Z);
                    var tangent = normal.Cross(Vector3.Up);

                    vertices[i] = vertex;
                    normals[i] = normal;
                    tangents[4 * i] = tangent.X;
                    tangents[4 * i + 1] = tangent.Y;
                    tangents[4 * i + 2] = tangent.Z;
                }

                arrays[(int)ArrayType.Vertex] = vertices;
                arrays[(int)ArrayType.Normal] = normals;
                arrays[(int)ArrayType.Tangent] = tangents;
                return ct.Cancelled() ? null : arrays;

                Vector3 GetNormal(float x, float z)
                {
                    var e = GetHeight(x + epsilon, z);
                    var w = GetHeight(x - epsilon, z);
                    var n = GetHeight(x, z + epsilon);
                    var s = GetHeight(x, z - epsilon);

                    var dx = (e - w) / epsilon2;
                    var dz = (n - s) / epsilon2;

                    return new Vector3(dx, 1, dz).Normalized();
                }
            }
        }

        void OnMeshReady(ArrayMesh mesh)
        {
            source.Mesh = mesh;
            Ready?.Invoke();
        }
    }
}
