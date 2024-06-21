using System.Linq;
using Godot;

namespace F00F;

public static class MeshExtensions_Clip
{
    public static MeshInstance3D Clip(this MeshInstance3D source, CollisionShape3D shape)
    {
        var copy = source.Copy();
        if (!Ok(out var sourceMesh, out var bb)) return copy;
        copy.Mesh = sourceMesh.Clip(bb);
        return copy;

        bool Ok(out ArrayMesh am, out Aabb bb)
        {
            am = source.Mesh as ArrayMesh;
            bb = shape.Shape.GetAabb().Grow(shape.Shape.Margin);
            if (am is null) { GD.PushWarning("MeshClip:  Mesh requires ArrayMesh - Using original mesh"); return false; }
            return true;
        }
    }

    // Currently only clips to bb, the idea being to separate disjoint parts
    private static ArrayMesh Clip(this ArrayMesh sourceMesh, in Aabb bb)
    {
        var targetMesh = new ArrayMesh();
        var surfaceCount = sourceMesh.GetSurfaceCount();
        for (var surface = 0; surface < surfaceCount; ++surface)
        {
            var targetData = new SurfaceTool();
            var sourceData = new MeshDataTool();
            targetData.Begin(Mesh.PrimitiveType.Triangles);
            sourceData.CreateFromSurface(sourceMesh, surface);

            var faceCount = sourceData.GetFaceCount();
            for (var face = 0; face < faceCount; ++face)
            {
                var x = new[] { 0, 1, 2 };

                var faceIndices = x.Select(x => sourceData.GetFaceVertex(face, x)).ToArray();
                var faceVertices = faceIndices.Select(sourceData.GetVertex).ToArray();

                if (faceVertices.Any(bb.HasPoint))
                {
                    var faceUV = faceIndices.Select(sourceData.GetVertexUV).ToArray();
                    var faceUV2 = faceIndices.Select(sourceData.GetVertexUV2).ToArray();
                    var faceColor = faceIndices.Select(sourceData.GetVertexColor).ToArray();
                    var faceNormal = faceIndices.Select(sourceData.GetVertexNormal).ToArray();
                    var faceTangent = faceIndices.Select(sourceData.GetVertexTangent).ToArray();

                    x.ForEach(x =>
                    {
                        targetData.SetUV(faceUV[x]);
                        targetData.SetUV2(faceUV2[x]);
                        targetData.SetColor(faceColor[x]);
                        targetData.SetNormal(faceNormal[x]);
                        targetData.SetTangent(faceTangent[x]);

                        targetData.AddVertex(faceVertices[x]);
                    });
                }
            }

            targetData.SetMaterial(sourceData.GetMaterial());
            targetData.Commit(targetMesh);
        }

        return targetMesh;
    }
}
