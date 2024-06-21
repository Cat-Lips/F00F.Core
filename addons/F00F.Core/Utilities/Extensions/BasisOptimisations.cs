//using System.Runtime.CompilerServices;
//using Godot;

//namespace F00F;

//public static class BasisOptimisations
//{
//    private const float Epsilon = 1e-6f;

//    #region Determinant

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static float GetD(this in Basis b)
//    {
//        return b.Row0.X * (b.Row1.Y * b.Row2.Z - b.Row1.Z * b.Row2.Y) +
//               b.Row0.Y * (b.Row1.Z * b.Row2.X - b.Row1.X * b.Row2.Z) +
//               b.Row0.Z * (b.Row1.X * b.Row2.Y - b.Row1.Y * b.Row2.X);
//    }

//    #endregion

//    #region IsScaled

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static bool IsScaled(this in Basis basis)
//        => basis.IsScaled(out var _);

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static bool IsScaled(this in Basis basis, float d)
//        => Mathf.Abs(Mathf.Abs(d) - 1.0f) > Epsilon;

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static bool IsScaled(this in Basis basis, out float d)
//        => basis.IsScaled(d = basis.GetD());

//    #endregion

//    #region GetScale

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Vector3 GetScale(this in Basis basis)
//        => basis.GetScale(out var _);

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Vector3 GetScale(this in Basis basis, float d)
//    {
//        if (!basis.IsScaled(d)) return Vector3.One;

//        var c0 = Mathf.Sqrt(basis.Row0.X * basis.Row0.X + basis.Row1.X * basis.Row1.X + basis.Row2.X * basis.Row2.X);
//        var c1 = Mathf.Sqrt(basis.Row0.Y * basis.Row0.Y + basis.Row1.Y * basis.Row1.Y + basis.Row2.Y * basis.Row2.Y);
//        var c2 = Mathf.Sqrt(basis.Row0.Z * basis.Row0.Z + basis.Row1.Z * basis.Row1.Z + basis.Row2.Z * basis.Row2.Z);

//        float sign = Mathf.Sign(d);
//        return new Vector3(c0 * sign, c1 * sign, c2 * sign);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Vector3 GetScale(this in Basis basis, out float d)
//        => basis.GetScale(d = basis.GetD());

//    #endregion

//    #region SetScale

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void SetScale(this ref Basis basis, in Vector3 scale)
//    {
//        basis.Row0 = new Vector3(basis.Row0.X * scale.X, basis.Row0.Y * scale.X, basis.Row0.Z * scale.X);
//        basis.Row1 = new Vector3(basis.Row1.X * scale.Y, basis.Row1.Y * scale.Y, basis.Row1.Z * scale.Y);
//        basis.Row2 = new Vector3(basis.Row2.X * scale.Z, basis.Row2.Y * scale.Z, basis.Row2.Z * scale.Z);
//    }

//    #endregion

//    #region ToGlobal

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Vector3 ToGlobal(this in Basis basis, in Vector3 localDirection)
//    {
//        return new Vector3(
//            basis.Row0.X * localDirection.X + basis.Row0.Y * localDirection.Y + basis.Row0.Z * localDirection.Z,
//            basis.Row1.X * localDirection.X + basis.Row1.Y * localDirection.Y + basis.Row1.Z * localDirection.Z,
//            basis.Row2.X * localDirection.X + basis.Row2.Y * localDirection.Y + basis.Row2.Z * localDirection.Z
//        );
//    }

//    #endregion
//}
