using System;
using Godot;

namespace F00F;

public static class VectorExtensions
{
    public static Vector2 V2(this in Vector3 self) => self.XZ();
    public static Vector2 Vec2(this in Vector3 self) => self.XZ();
    public static Vector2 XZ(this in Vector3 self) => new(self.X, self.Z);

    public static Vector3 V3(this in Vector2 self, float y = 0) => self.FromXZ(y);
    public static Vector3 Vec3(this in Vector2 self, float y = 0) => self.FromXZ(y);
    public static Vector3 FromXZ(this in Vector2 self, float y = 0) => new(self.X, y, self.Y);

    public static bool IsZero(this in Vector2 self) => self.X is 0 && self.Y is 0;
    public static bool IsZero(this in Vector3 self) => self.X is 0 && self.Y is 0 && self.Z is 0;
    public static bool NotZero(this in Vector2 self) => !self.IsZero();
    public static bool NotZero(this in Vector3 self) => !self.IsZero();

    public static Vector3 With(this in Vector3 self, float? x = null, float? y = null, float? z = null)
        => new(x ?? self.X, y ?? self.Y, z ?? self.Z);

    public static float[] ToArray(this in Vector3 self)
        => [self.X, self.Y, self.Z];

    public static bool IsUniform(this in Vector3 self, float tolerance = Const.TinyFloat)
    {
        return Mathf.IsEqualApprox(self.X, self.Y, tolerance) &&
               Mathf.IsEqualApprox(self.X, self.Z, tolerance);
    }

    public static float GetUniformValue(this in Vector3 self, float tolerance = Const.TinyFloat)
    {
        return self.IsUniform(tolerance)
            ? self.ToArray().GetMostRoundValue()
            : throw new Exception($"Expected uniform XYZ in Vec3: {self}");
    }

    public static Vector3 Unify(this in Vector3 self, float tolerance = Const.TinyFloat)
        => Vector3.One * self.GetUniformValue(tolerance);

    public static Vector2 GetVelocityTo(this in Vector2 self, in Vector2 to, float friction)
    {
        var direction = self.DirectionTo(to);
        var distance = self.DistanceTo(to);
        var force = Mathf.Sqrt(2 * distance * friction);
        return direction * force;
    }

    public static Vector3 GetVelocityTo(this in Vector3 self, in Vector3 to, float friction)
    {
        var direction = self.DirectionTo(to);
        var distance = self.DistanceTo(to);
        var force = Mathf.Sqrt(2 * distance * friction);
        return direction * force;
    }

    public static (Vector2 Velocity, float HeightVelocity) GetVelocityTo(this in Vector2 self, in Vector2 to, float friction, float lowDistLimit, float endHeightIncrease = 0)
    {
        var direction = self.DirectionTo(to);
        var distance = self.DistanceTo(to);

        if (distance <= lowDistLimit)
        {
            var force = Mathf.Sqrt(2 * distance * friction);
            return (direction * force, 0f);
        }
        else
        {
            var force = Mathf.Sqrt(2 * distance * friction);
            var upForce = Const.Gravity * distance / ((2 - endHeightIncrease) * force);
            return (direction * force, upForce);
        }
    }

    public static Vector3 AlignWith(this in Vector3 self, in Vector3 up)
        => self - up * self.Dot(up);
}
