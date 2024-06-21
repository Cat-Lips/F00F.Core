using Godot;

namespace F00F;

using Camera = Godot.Camera3D;

public static class CameraExtensions_3D
{
    public static Shape3D GetSphereShape(this Camera camera, float expand = 0)
    {
        camera.NearPlane(out var width, out var height, out var near, expand);

        var corner = new Vector3(width, height, -near);

        return new SphereShape3D()
        {
            Radius = corner.Length()
        };
    }

    public static Shape3D GetPyramidShape(this Camera camera, float expand = 0)
    {
        camera.NearPlane(out var width, out var height, out var near, expand);

        return new ConvexPolygonShape3D()
        {
            Points =
            [
                Vector3.Zero,
                new(-width,  height, -near),
                new( width,  height, -near),
                new(-width, -height, -near),
                new( width, -height, -near),
            ]
        };
    }

    private static void NearPlane(this Camera camera, out float width, out float height, out float near, float expand)
    {
        var fov = Mathf.DegToRad(camera.Fov);
        var aspect = camera.GetDisplayRect().Size.Aspect();

        var halfTanFov = Mathf.Tan(fov * .5f);

        near = camera.Near;
        height = halfTanFov * near + expand;
        width = height * aspect + expand;

    }
}
