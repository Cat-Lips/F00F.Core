using Godot;

namespace F00F;

public static class Transform3DExtensions
{
    public static Transform3D SetPosition(this in Transform3D source, in Vector3 position)
        => new(source.Basis, position);

    public static Transform3D SetRotation(this in Transform3D source, in Vector3 axis, float angle)
        => new(new Basis(axis, angle).Scaled(source.Basis.Scale), source.Origin);

    public static Transform3D SetScale(this in Transform3D source, in Vector3 scale)
        => new(source.Basis.Orthonormalized().Scaled(scale), source.Origin);

    public static Transform3D TrimPosition(this in Transform3D source)
        => new(source.Basis, Vector3.Zero);

    public static Transform3D TrimRotation(this in Transform3D source)
        => new(Basis.Identity.Scaled(source.Basis.Scale), source.Origin);

    public static Transform3D TrimScale(this in Transform3D source)
        => new(source.Basis.Orthonormalized(), source.Origin);

    public static Transform3D GetPositionTransform(this in Transform3D source)
        => new(Basis.Identity, source.Origin);

    public static Transform3D GetRotationTransform(this in Transform3D source)
        => new(source.Basis.Orthonormalized(), Vector3.Zero);

    public static Transform3D GetScaleTransform(this in Transform3D source)
        => new(Basis.Identity.Scaled(source.Basis.Scale), Vector3.Zero);

    public static Aabb Transform(this in Transform3D xform, in Aabb x) => xform * x;
    public static Aabb TransformedBy(this in Aabb x, in Transform3D xform) => xform * x;
    public static Vector3 Transform(this in Transform3D xform, in Vector3 x) => xform * x;
    public static Vector3 TransformedBy(this in Vector3 x, in Transform3D xform) => xform * x;
    public static Transform3D Transform(this in Transform3D xform, in Transform3D x) => xform * x;
    public static Transform3D TransformedBy(this in Transform3D x, in Transform3D xform) => xform * x;

    public static Basis Transform(this in Basis basis, in Basis x) => basis * x;
    public static Basis TransformedBy(this in Basis x, in Basis basis) => basis * x;
    public static Vector3 Transform(this in Basis basis, in Vector3 x) => basis * x;
    public static Vector3 TransformedBy(this in Vector3 x, in Basis basis) => basis * x;

    public static void Transform<T>(this in Transform3D xform, T x) where T : Node3D => x.Transform = xform.Transform(x.Transform);
    public static void TransformBy<T>(this T x, in Transform3D xform) where T : Node3D => x.Transform = x.Transform.TransformedBy(xform);
    public static void TransformGlobal<T>(this in Transform3D xform, T x) where T : Node3D => x.GlobalTransform = xform.Transform(x.GlobalTransform);
    public static void TransformGlobalBy<T>(this T x, in Transform3D xform) where T : Node3D => x.GlobalTransform = x.GlobalTransform.TransformedBy(xform);

    public static Transform3D AlignWith(this Transform3D source, in Vector3 up, bool normalise = false)
    {
        source.Basis.Y = up;
        source.Basis.X = source.Fwd().Cross(up);
        return normalise ? source.Orthonormalized() : source;
    }

    public static void Get(this in Transform3D self, out Vector3 up, out Vector3 fwd, out Vector3 side, out Vector3 pos)
    {
        up = self.Up();
        fwd = self.Fwd();
        side = self.Side();
        pos = self.Pos();
    }
}
