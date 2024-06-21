using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace F00F;

using Body = CollisionObject3D;
using Shape = CollisionShape3D;

[Tool]
public partial class SafeSpawn : Area3D
{
    private Shape SafeZone { get; set; }
    private Queue<Body> BodyQ { get; } = new();

    #region Export

    [Export] public NodePath SpawnParent { get; set; } = ".";

    #endregion

    public void QueueSpawn(Body body)
        => BodyQ.Enqueue(body);

    public void SpawnNow(Body body)
    {
        body.TopLevel = true;
        body.Position = GlobalPosition;
        GetNode(SpawnParent).AddChild(body, own: true);
    }

    public void ResetSpawn()
    {
        while (BodyQ.Count > 0)
            BodyQ.Dequeue().QueueFree();
        if (SafeZone is null) return;
        this.RemoveChild(SafeZone, free: true);
        SafeZone = null;
    }

    #region Godot

    public sealed override void _PhysicsProcess(double _)
    {
        if (BodyQ.TryPeek(out var body))
            TrySpawn(body);

        void TrySpawn(Body body)
        {
            if (SafeZone is null)
            {
                AddChild(SafeZone = body.GetAabb().CreateBoxShape(), @internal: InternalMode.Front);
                Debug.Assert(GetOverlappingBodies().Count is 0);
                return;
            }

            var overlap = GetOverlappingBodies();
            if (overlap.Count is 0)
            {
                this.RemoveChild(SafeZone, free: true);
                SafeZone = null;
                SpawnNow(body);

                var _body = BodyQ.Dequeue();
                Debug.Assert(body == _body);

                return;
            }
        }
    }

    #endregion
}
