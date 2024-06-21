using Godot;

namespace F00F;
public static class CharacterBodyExtensions
{
    private const float PushMultiplier = 5;
    private const float PushRatioThreshold = .25f;
    public static float DefaultMass { get; set; } = 80f;

    public static void ApplyMoveAndSlide(this CharacterBody3D self, Vector3? up = null)
    {
        ApplyPhysics();
        self.MoveAndSlide();

        void ApplyPhysics()
        {
            var count = self.GetSlideCollisionCount();
            for (var i = 0; i < count; ++i)
            {
                var state = self.GetSlideCollision(i);
                self.ApplyPhysics(state, up);
            }
        }
    }

    public static void ApplyMoveAndCollide(this CharacterBody3D self, in Vector3 motion, Vector3? up = null, int max = 1)
    {
        var state = self.MoveAndCollide(motion, maxCollisions: max);
        self.ApplyPhysics(state, up);
    }

    private static void ApplyPhysics(this CharacterBody3D self, KinematicCollision3D state, Vector3? up = null)
    {
        state?.GetCollisions(out var depth, out var travel, out var remainder, up).ForEach(x =>
        {
            if (x.Other is RigidBody3D body)
            {
                var pushDir = -x.ImpactNormal;
                var myVelocity = self.Velocity;
                var bodyVelocity = body.LinearVelocity;
                var myVelocityInPushDirection = myVelocity.Dot(pushDir);
                var bodyVelocityInPushDirection = bodyVelocity.Dot(pushDir);
                var pushPower = Mathf.Max(0, myVelocityInPushDirection - bodyVelocityInPushDirection);

                var myMass = (float)self.GetMeta(GLB.Mass, DefaultMass);
                var massRatio = Mathf.Min(1, myMass / body.Mass);
                if (massRatio < PushRatioThreshold) return;
                pushDir.Y = 0;

                var pushForce = pushDir * pushPower * massRatio * PushMultiplier;
                var pushPosition = x.ImpactPosition - body.GlobalPosition;
                body.ApplyImpulse(pushForce, pushPosition);
                //body.DrawImpulse(pushForce, pushPosition);
                return;
            }

            if (x.Other is CharacterBody3D other)
            {
                var pushDir = -x.ImpactNormal;
                var myVelocity = self.Velocity;
                var bodyVelocity = other.Velocity;
                var myVelocityInPushDirection = myVelocity.Dot(pushDir);
                var bodyVelocityInPushDirection = bodyVelocity.Dot(pushDir);
                var pushPower = Mathf.Max(0, myVelocityInPushDirection - bodyVelocityInPushDirection);

                var myMass = (float)self.GetMeta(GLB.Mass, DefaultMass);
                var bodyMass = (float)other.GetMeta(GLB.Mass, DefaultMass);
                var massRatio = Mathf.Min(1, myMass / bodyMass);
                pushDir.Y = 0;

                var pushForce = pushDir * pushPower * massRatio * PushMultiplier;
                //var pushPosition = x.ImpactPosition - other.GlobalPosition;
                other.Velocity += pushForce;
                return;
            }
        });
    }

    //public static void PhysicsSync(this CharacterBody3D self, params IEnumerable<MeshInstance3D> mesh)
    //{
    //    var x = Engine.GetPhysicsInterpolationFraction();
    //    mesh.ForEach(mesh.Transform);
    //    //var fps = (float)Engine.GetFramesPerSecond();
    //    //var lerp = self.Velocity / fps;
    //    //var pos = self.GlobalPosition + lerp;

    //    //if (fps > Engine.PhysicsTicksPerSecond)
    //    //{

    //    //}
    //}
}
