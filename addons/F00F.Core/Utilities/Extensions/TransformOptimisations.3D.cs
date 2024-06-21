//using System.Runtime.CompilerServices;
//using Godot;

//namespace F00F;

//public static class TransformOptimisations_3D
//{
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void SetRotation(this ref Transform3D gform, in Vector3 globalRotation)
//    {
//        if (!gform.Basis.IsScaled(out var d)) { gform.SetRotationUnscaled(globalRotation); return; }

//        var scale = gform.Basis.GetScale(d);
//        gform.Basis = Basis.FromEuler(globalRotation);
//        gform.Basis.SetScale(scale);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void SetRotationUnscaled(this ref Transform3D gform, in Vector3 globalRotation)
//        => gform.Basis = Basis.FromEuler(globalRotation);

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void ApplyMovement(this ref Transform3D gform, in Vector3 localMovement)
//    {
//        if (localMovement.IsZero()) return;

//        var x = gform.Basis.Row0.X * localMovement.X + gform.Basis.Row0.Y * localMovement.Y + gform.Basis.Row0.Z * localMovement.Z;
//        var y = gform.Basis.Row1.X * localMovement.X + gform.Basis.Row1.Y * localMovement.Y + gform.Basis.Row1.Z * localMovement.Z;
//        var z = gform.Basis.Row2.X * localMovement.X + gform.Basis.Row2.Y * localMovement.Y + gform.Basis.Row2.Z * localMovement.Z;

//        gform.Origin.X += x;
//        gform.Origin.Y += y;
//        gform.Origin.Z += z;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void Transform(this ref Transform3D gform, in Vector3 globalRotation, in Vector3 localMovement)
//    {
//        gform.SetRotation(globalRotation);
//        gform.ApplyMovement(localMovement);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static void TransformUnscaled(this ref Transform3D gform, in Vector3 globalRotation, in Vector3 localMovement)
//    {
//        gform.SetRotationUnscaled(globalRotation);
//        gform.ApplyMovement(localMovement);
//    }

//    #region ToGlobal

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Vector3 ToGlobal(this in Transform3D gform, in Vector3 localPosition)
//    {
//        return new Vector3(
//            gform.Basis.Row0.X * localPosition.X + gform.Basis.Row0.Y * localPosition.Y + gform.Basis.Row0.Z * localPosition.Z + gform.Origin.X,
//            gform.Basis.Row1.X * localPosition.X + gform.Basis.Row1.Y * localPosition.Y + gform.Basis.Row1.Z * localPosition.Z + gform.Origin.Y,
//            gform.Basis.Row2.X * localPosition.X + gform.Basis.Row2.Y * localPosition.Y + gform.Basis.Row2.Z * localPosition.Z + gform.Origin.Z
//        );
//    }

//    #endregion
//}
