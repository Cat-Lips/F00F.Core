using Godot;

namespace F00F
{
    public static class AabbExtensions
    {
        public static Aabb TransformedBy(this Aabb bb, in Transform3D xform)
            => xform * bb;

        //public static Aabb ScaledBy(this Aabb bb, in Vector3 scale)
        //{
        //    bb.Position *= -scale * .5f;
        //    bb.Size *= scale;
        //    return bb.Abs();
        //}

        //public static Aabb Grow(this Aabb bb, in Vector3 by)
        //{
        //    bb.Position -= by;
        //    bb.Size += by * 2;
        //    return bb.Abs();
        //}
    }
}
