using Godot;

namespace F00F
{
    public static class Transform3DExtensions
    {
        public static Transform3D TrimPosition(this in Transform3D source)
            => new(source.Basis, Vector3.Zero);

        public static Transform3D TrimRotation(this in Transform3D source)
            => new(Basis.Identity.Scaled(source.Basis.Scale), source.Origin);

        public static Transform3D TrimScale(this in Transform3D source)
            => new(source.Basis.Orthonormalized(), source.Origin);

        public static Transform3D Position(this in Transform3D source)
            => new(Basis.Identity, source.Origin);

        public static Transform3D Rotation(this in Transform3D source)
            => new(source.Basis.Orthonormalized(), Vector3.Zero);

        public static Transform3D Scale(this in Transform3D source)
            => new(Basis.Identity.Scaled(source.Basis.Scale), Vector3.Zero);

        public static Transform3D Align(this Transform3D source, in Vector3 up)
        {
            source.Basis.Y = up;
            source.Basis.X = -source.Basis.Z.Cross(up);
            return source.Orthonormalized();
        }
    }
}
