using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

namespace F00F;

[Obsolete("Not reliable - Use ShapeCastExtensions_PhysicsServer instead")]
public static class ShapeCastExtensions_PhysicsState
{
    public static bool MoveAndSlide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, params Node3D[] exclude)
        => world.CastMotion(shape, basis, from, ref to, OnCollision.Slide, 6, exclude);

    public static bool MoveAndCollide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, params Node3D[] exclude)
        => world.CastMotion(shape, basis, from, ref to, OnCollision.Stop, 0, exclude);

    public static bool MoveAndSlide(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, uint slides, params Node3D[] exclude)
        => world.CastMotion(shape, basis, from, ref to, OnCollision.Slide, slides, exclude);

    public enum PushBack { Backup, Backout } // Backup along gform.Back(), Backout along collision normal
    public static bool Collide(this World3D world, Shape3D shape, ref Transform3D gform, PushBack strategy, params Node3D[] exclude)
        => world.Collide(shape, ref gform, strategy, out var _, exclude);

    public static bool Collide(this World3D world, Shape3D shape, ref Transform3D gform, PushBack strategy, out Vector3 push, params Node3D[] exclude)
    {
        push = default;
        if (shape is null) return false;
        Debug.Assert(Engine.IsInPhysicsFrame());
        return world.DirectSpaceState.CollideShape(shape, ref gform, strategy, out push, exclude);
    }

    public static bool CollisionNormal(this World3D world, Shape3D shape, ref Transform3D gform, out Vector3 normal, params Node3D[] exclude)
    {
        var ret = world.Collide(shape, ref gform, PushBack.Backout, out var push, exclude);
        normal = ret ? push.Normalized() : default;
        return ret;
    }

    #region Private

    private enum OnCollision { Stop, Slide }
    private static bool CastMotion(this World3D world, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, OnCollision strategy, uint slides, params Node3D[] exclude)
    {
        if (shape is null) return false;
        Debug.Assert(Engine.IsInPhysicsFrame());
        return world.DirectSpaceState.CastMotion(shape, basis, from, ref to, strategy, slides, exclude);
    }

    #region Base

    private static bool CastMotion(this PhysicsDirectSpaceState3D pdss, Shape3D shape, in Basis basis, in Vector3 from, ref Vector3 to, OnCollision strategy, uint slides, params Node3D[] exclude)
    {
        GD.Print($" *0 CastMotion [from: {from}, to: {to}, motion: {to - from} ({(to - from).Length()})]");

        var gform = new Transform3D(basis, from);
        if (pdss.CollideShape(shape, ref gform, PushBack.Backout, out var _push, exclude))
            GD.Print($" *1 Pre-CollideShape [from: {gform.Origin}, motion: {to - gform.Origin} ({(to - gform.Origin).Length()}), push: {_push} ({_push.Length()})]");

        var motion = to - gform.Origin;
        var (safe, @unsafe) = pdss.CastMotion(gform, motion);
        GD.Print($" - [safe: {safe}, unsafe: {@unsafe}]");

        if (safe is 1)
        {
            gform.Origin = to;
            var ret = pdss.CollideShape(shape, ref gform, PushBack.Backout, out _push, exclude);
            if (ret) GD.Print($" *1 Post-CollideShape [to.before: {to}, to.after: {gform.Origin}, push: {_push} ({_push.Length()})]");
            to = gform.Origin;

            if (to.Y <= 0) GD.Print("HERE (post-collide-shape)");

            return ret;
        }

        switch (strategy)
        {
            case OnCollision.Stop: StopTarget(ref to); return true;
            case OnCollision.Slide: SlideTarget(ref to); return true;
            default: throw new NotImplementedException();
        }

        void StopTarget(ref Vector3 to)
            => to = gform.Origin + motion * safe;

        void SlideTarget(ref Vector3 to)
        {
            var safePos = gform.Origin;

            while (true)
            {
                GD.Print($" - [slides: {slides}, safePos: {safePos.Y}]");

                gform.Origin = safePos + motion * @unsafe;
                pdss.CollideShape(shape, ref gform, PushBack.Backout, out var push, exclude);
                if (!push.TryNormalise(out var normal)) break;

                GD.Print($" - [slides: {slides}, normal: {normal}, push: {push.Y}]");

                safePos += motion * safe;
                var remainder = motion * (1f - safe);
                motion = remainder.Slide(normal);

                GD.Print($" - slides: {slides}, remainder: {remainder} ({remainder.Length()}), motion (new): {motion} ({motion.Length()})");

                gform.Origin = safePos;
                (safe, @unsafe) = pdss.CastMotion(gform, motion);
                GD.Print($" - [safe: {safe}, unsafe: {@unsafe}]");

                if (safe is 1) { safePos += motion; break; }
                if (--slides <= 0) break;
                if (safe is 0) break;
            }

            GD.Print($" - DONE [safePos: {safePos.Y}{" ***HERE***".StrIf(safePos.Y <= 0)}]");

            gform.Origin = safePos;
            var ret = pdss.CollideShape(shape, ref gform, PushBack.Backout, out _push, exclude);
            if (ret) GD.Print($" *1 PostSlide-CollideShape [safePos.before: {safePos.Y}, safePos.after: {gform.Origin.Y}, push: {_push.Y}]");
            to = gform.Origin;

            GD.Print($" - DONE [to: {to.Y}{" ***HERE***".StrIf(to.Y <= 0)}]");
        }
    }

    private static bool CollideShape(this PhysicsDirectSpaceState3D pdss, Shape3D shape, ref Transform3D gform, PushBack strategy, out Vector3 push, params Node3D[] exclude)
    {
        var colls = pdss.CollideShape(shape, gform, exclude);
        if (colls.Count is 0) { push = default; return false; }
        switch (strategy)
        {
            case PushBack.Backup: BackupTarget(ref gform, out push); return true;
            case PushBack.Backout: BackoutTarget(ref gform, out push); return true;
            default: throw new NotImplementedException();
        }

        void BackupTarget(ref Transform3D gform, out Vector3 push)
        {
            gform.Origin += push = gform.Back() * GetBackup(gform);

            float GetBackup(in Transform3D gform)
            {
                var maxPush = 0f;
                for (var i = 0; i < colls.Count; i += 2)
                {
                    var shapeCol = colls[i];
                    var otherCol = colls[i + 1];
                    var pushVector = otherCol - shapeCol;
                    var pushBack = gform.Back(pushVector);

                    if (pushBack > maxPush)
                        maxPush = pushBack;
                }

                return maxPush;
            }
        }

        void BackoutTarget(ref Transform3D gform, out Vector3 push)
        {
            gform.Origin += push = GetBackout();

            Vector3 GetBackout()
            {
                var maxLength = 0f;
                var maxPush = Vector3.Zero;
                for (var i = 0; i < colls.Count; i += 2)
                {
                    var shapeCol = colls[i];
                    var otherCol = colls[i + 1];
                    var pushVector = otherCol - shapeCol;
                    var pushLength = pushVector.LengthSquared();
                    if (pushLength > maxLength)
                    {
                        maxPush = pushVector;
                        maxLength = pushLength;
                    }
                }

                return maxPush;
            }
        }
    }

    #endregion

    #region Core

    private static Array<Vector3> CollideShape(this PhysicsDirectSpaceState3D pdss, in Transform3D gform)
        => pdss.CollideShape(Query.Get(gform));

    private static (float Safe, float Unsafe) CastMotion(this PhysicsDirectSpaceState3D pdss, in Transform3D gform, in Vector3 motion)
    {
        var result = pdss.CastMotion(Query.Get(gform, motion));
        return (result[0], result[1]);
    }

    private static Array<Vector3> CollideShape(this PhysicsDirectSpaceState3D pdss, Shape3D shape, in Transform3D gform, params Node3D[] exclude)
        => pdss.CollideShape(Query.Get(shape, gform, exclude));

    private static (float Safe, float Unsafe) CastMotion(this PhysicsDirectSpaceState3D pdss, Shape3D shape, in Transform3D gform, in Vector3 motion, params Node3D[] exclude)
    {
        var result = pdss.CastMotion(Query.Get(shape, gform, motion, exclude));
        return (result[0], result[1]);
    }

    #endregion

    #region Query

    private static class Query
    {
        [ThreadStatic]
        private static PhysicsShapeQueryParameters3D _query;
        private static PhysicsShapeQueryParameters3D Get() => _query ??= new()
        {
            CollisionMask = 1,
            CollideWithAreas = false,
            CollideWithBodies = true,
        };

        public static PhysicsShapeQueryParameters3D Get(in Transform3D gform)
        {
            var query = Get();
            query.Transform = gform;
            return query;
        }

        public static PhysicsShapeQueryParameters3D Get(in Transform3D gform, in Vector3 motion)
        {
            var query = Get();
            query.Motion = motion;
            query.Transform = gform;
            return query;
        }

        public static PhysicsShapeQueryParameters3D Get(Shape3D shape, in Transform3D gform, params Node3D[] exclude)
        {
            var query = Get();
            query.Shape = shape;
            query.Exclude = exclude.Rids();
            query.Transform = gform;
            return query;
        }

        public static PhysicsShapeQueryParameters3D Get(Shape3D shape, in Transform3D gform, in Vector3 motion, params Node3D[] exclude)
        {
            var query = Get();
            query.Shape = shape;
            query.Motion = motion;
            query.Exclude = exclude.Rids();
            query.Transform = gform;
            return query;
        }
    }

    #endregion

    #endregion
}
