using Godot;

namespace F00F;

public static class MeshExtensions_Mesh
{
    public static MeshInstance3D AsMeshInstance(this Mesh source, string name = "Mesh")
        => new() { Name = name, Mesh = source };

    public static MeshInstance3D AsMeshInstance(this Mesh source, in Transform3D xform, string name = "Mesh")
        => new() { Name = name, Mesh = source, Transform = xform };

    //public static MultiMeshInstance3D AsMultiMeshInstance(this Mesh source, string name = "Mesh")
    //    => new() { Name = name, Mesh = source };
}
