using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace F00F;

public static class ShapeCastExtensions_PhysicsServer
{
    #region Private

    private const uint DefaultMaxSlides = 6;
    private const float DefaultFloorRange = Const.OneCentimeter;

    #endregion

    public static bool SafeMoveAndSlide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, params Node3D[] exclude)
        => world.TestMotion(shape, basis, from, ref to, DefaultMaxSlides, exclude);

    public static bool SafeMoveAndSlide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, uint maxSlides, params Node3D[] exclude)
        => world.TestMotion(shape, basis, from, ref to, maxSlides, exclude);

    public static bool SafeMoveAndCollide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, params Node3D[] exclude)
        => world.TestMotion(shape, basis, from, ref to, 0, exclude);

    public static bool IsOnFloor(this CollisionShape3D shape, out Vector3 normal)
        => shape.IsOnFloor(DefaultFloorRange, out normal);

    public static bool IsOnFloor(this CollisionShape3D shape, float range, out Vector3 normal)
    {
        var world = shape.GetWorld3D();
        var gform = shape.GlobalTransform;
        var motion = gform.Down() * range;
        var result = world.TestMotion(shape.Shape, gform, motion, shape.GetParentNode3D());
        if (result is null) { normal = Vector3.Up; return false; }

        normal = result.GetCollisionNormal();
        return gform.IsUp(normal);
    }

    #region Private

    private static bool TestMotion(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, uint slides, params Node3D[] exclude)
    {
        var result = world.TestMotion(shape, new(basis, from), to - from, exclude);
        if (result is null) return false;
        if (slides is 0) StopTarget(from, ref to);
        else SlideTarget(basis, from, ref to);
        return true;

        void StopTarget(in Vector3 from, ref Vector3 to)
            => to = from + result.GetTravel();

        void SlideTarget(in Basis basis, in Vector3 from, ref Vector3 to)
        {
            var safePos = from + result.GetTravel();

            while (slides > 0)
            {
                var remainder = result.GetRemainder();
                var normal = result.GetCollisionNormal();
                var motion = remainder.Slide(normal);

                result = _TestMotion(new(basis, safePos), motion);
                if (result is null) { safePos += motion; break; }
                safePos += result.GetTravel();

                --slides;
            }

            to = safePos;
        }
    }

    #region Core

    private static Rid curBody;
    private static PhysicsTestMotionResult3D curResult;
    private static PhysicsTestMotionParameters3D curQuery;

    private static PhysicsTestMotionResult3D TestMotion(this World3D world, Shape3D shape, in Transform3D from, in Vector3 motion, params Node3D[] exclude)
    {
        Query.Get(world, shape, out curBody, out curQuery, out curResult);

        curQuery.From = from;
        curQuery.Motion = motion;
        curQuery.ExcludeBodies = exclude.Rids();

        Debug.Assert(Engine.IsInPhysicsFrame());
        return PhysicsServer3D.BodyTestMotion(curBody, curQuery, curResult) ? curResult : null;
    }

    private static PhysicsTestMotionResult3D _TestMotion(in Transform3D from, in Vector3 motion)
    {
        curQuery.From = from;
        curQuery.Motion = motion;

        return PhysicsServer3D.BodyTestMotion(curBody, curQuery, curResult) ? curResult : null;
    }

    #endregion

    #region Query

    private static class Query
    {
        [ThreadStatic]
        private static Dictionary<(Rid World, Rid Shape), Rid> _body;
        private static Rid GetBody(Rid world, Rid shape)
        {
            if (!(_body ??= []).TryGetValue((world, shape), out var body))
                _body.Add((world, shape), body = NewBody());
            return body;

            Rid NewBody()
            {
                var body = PhysicsServer3D.BodyCreate();
                PhysicsServer3D.BodySetSpace(body, world);
                PhysicsServer3D.BodyAddShape(body, shape);
                PhysicsServer3D.BodySetCollisionLayer(body, 0);
                return body;
            }
        }

        [ThreadStatic]
        private static PhysicsTestMotionParameters3D _query;
        private static PhysicsTestMotionParameters3D GetQuery() => _query ??= new()
        {
            MaxCollisions = 1,
        };

        [ThreadStatic]
        private static PhysicsTestMotionResult3D _result;
        private static PhysicsTestMotionResult3D GetResult() => _result ??= new();

        public static void Get(World3D world, Shape3D shape, out Rid body, out PhysicsTestMotionParameters3D query, out PhysicsTestMotionResult3D result)
        {
            body = GetBody(world.Space, shape.GetRid());
            query = GetQuery();
            result = GetResult();
        }
    }

    #endregion

    #endregion
}
