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
    public static bool IsZeroExact(this in Vector2 self) => self.IsZero();
    public static bool IsZeroExact(this in Vector3 self) => self.IsZero();
    public static bool NotZeroExact(this in Vector2 self) => !self.IsZeroExact();
    public static bool NotZeroExact(this in Vector3 self) => !self.IsZeroExact();

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

    public static Vector3 ClosestPointOnLine(this in Vector3 point, in Vector3 lineFrom, in Vector3 lineTo)
    {
        var dir = lineTo - lineFrom;
        var sqrLen = dir.LengthSquared();
        if (sqrLen is 0f) return lineFrom; // (ie, lineFrom == lineTo)
        var t = Mathf.Clamp((point - lineFrom).Dot(dir) / sqrLen, 0f, 1f);
        return lineFrom.Lerp(lineTo, t);
    }

    public static Vector3 ProjectOnPlane(this in Vector3 self, in Vector3 normal)
        => self - normal * self.Dot(normal); // Same as Vector3.Slide(normal)

    public static Vector2I V2(this in Vector3I self) => self.XZ();
    public static Vector2I Vec2(this in Vector3I self) => self.XZ();
    public static Vector2I XZ(this in Vector3I self) => new(self.X, self.Z);

    public static Vector3I V3(this in Vector2I self, int y = 0) => self.FromXZ(y);
    public static Vector3I Vec3(this in Vector2I self, int y = 0) => self.FromXZ(y);
    public static Vector3I FromXZ(this in Vector2I self, int y = 0) => new(self.X, y, self.Y);

    public static bool TryNormalise(this in Vector3 self, out Vector3 normal, float tolerance = Const.Epsilon)
    {
        normal = self;
        var num = normal.LengthSquared();
        if (Mathf.IsEqualApprox(num, 0f, tolerance))
        {
            normal.X = normal.Y = normal.Z = 0f;
            return false;
        }

        var num2 = Mathf.Sqrt(num);
        normal.X /= num2;
        normal.Y /= num2;
        normal.Z /= num2;
        return true;
    }
}
