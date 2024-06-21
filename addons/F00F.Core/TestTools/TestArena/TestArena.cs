using System;
using Godot;

namespace F00F;

using static TestArenaConfig.Enum;
using Camera = Godot.Camera3D;

[Tool]
public partial class TestArena : Node3D
{
    #region Private

    private Camera Camera => field ??= GetViewport().GetCamera3D();
    private WorldBounds Bounds => field ??= (WorldBounds)GetNode("Bounds");

    private Node3D Stunts => field ??= (Node3D)GetNode("Stunts");

    private StaticBody3D Floor => field ??= (StaticBody3D)GetNode("Floor");
    private MeshInstance3D FloorMesh => field ??= (MeshInstance3D)Floor.GetNode("Mesh");
    private CollisionShape3D FloorShape => field ??= (CollisionShape3D)Floor.GetNode("Shape");
    private PlaneMesh FloorPlane => field ??= (PlaneMesh)FloorMesh.Mesh;

    private StaticBody3D Loop => field ??= (StaticBody3D)Stunts.GetNode("Loop");
    private MeshInstance3D LoopMesh => field ??= (MeshInstance3D)Loop.GetNode("Mesh");
    private CollisionShape3D LoopShape => field ??= (CollisionShape3D)Loop.GetNode("Shape");

    #endregion

    public event Action<Vector3> SpawnReady;
    public Vector3? SpawnPoint { get; private set; }

    #region Export

    [Export] public TestArenaConfig Config { get; set => this.Set(ref field, value ?? new(), OnConfigSet); }

    #endregion

    #region Godot

    public sealed override void _Ready()
    {
        InitLoop();
        Config ??= new();
        Editor.Disable(this);

        void InitLoop()
        {
            var mesh = New.StuntLoopMesh();
            var shape = New.StaticShape(mesh);

            LoopMesh.Mesh = mesh;
            LoopShape.Shape = shape;
        }
    }

    public sealed override void _Process(double _)
        => this.Clamp(Camera, Camera.Near); // FIXME: outer sphere radius!

    #endregion

    #region Private

    private void OnConfigSet()
    {
        if (this.IsReady())
        {
            ShowStunts();
            SetFloorSize();
            SetFloorShape();
            SetStuntsScale();
            Config.FloorSizeSet.Action += SetFloorSize;
            Config.FloorShapeSet.Action += SetFloorShape;
            Config.ShowStuntsSet.Action += ShowStunts;
            Config.StuntsScaleSet.Action += SetStuntsScale;
        }

        void SetFloorSize()
        {
            SetFloorSize();
            SetFloorShape();
            SetSpawnReady();

            void SetFloorSize()
            {
                Bounds.Size = Vector2I.One * Config.FloorSize;
                FloorPlane.Size = Vector2.One * Config.FloorSize;
            }

            void SetSpawnReady()
            {
                SpawnPoint = new(0, 0, Config.FloorSize * .25f);
                SpawnReady?.Invoke(SpawnPoint.Value);
            }
        }

        void SetFloorShape()
        {
            FloorShape.Shape = Config.FloorShape switch
            {
                ShapeType.Plane => new WorldBoundaryShape3D(),
                ShapeType.Polygon => New.ConvexPlane(Config.FloorSize),
                ShapeType.Trimesh => New.ConcavePlane(Config.FloorSize),
                _ => throw new NotImplementedException(),
            };
        }

        void ShowStunts()
            => Stunts.Enabled(Config.ShowStunts);

        void SetStuntsScale()
            => Stunts.Scale = Vector3.One * Config.StuntsScale;
    }

    #endregion
}
